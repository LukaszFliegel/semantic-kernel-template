using Microsoft.SemanticKernel;
using Perun.AI.ChatHistory;
using Perun.AI.Configuration;
using Perun.AI.SemanticKernel;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

builder.Services.AddScoped<ISemanticKernelFactory, SemanticKernelFactory>();
builder.Services.AddScoped<IKernelBuilderFactory, DefaultKernelBuilderFactory>();
builder.Services.AddScoped<Kernel>(sp =>
{
    var factory = sp.GetRequiredService<ISemanticKernelFactory>();
    return factory.CreateKernelWithPlugins();
});

// add you plugins here
//builder.Services.AddScoped<IKernelPlugin, SamplePerunBotKernelPlugin>();

builder.Services.AddSingleton<IChatStore, MemoryCacheChatStore>();
builder.Services.AddScoped<IChatHistoryCleaner, ChatHistoryCleaner>();
// to cut-off long chat history use this:
builder.Services.AddScoped<ILongChatHistoryHandler, LongChatHistoryCutOffHandler>();
// to summarize long chat history use this:
//builder.Services.AddScoped<ILongChatHistoryHandler, LongChatHistorySummarizeHandler>();

builder.Services.Configure<AzureOpenAiSettings>(builder.Configuration.GetSection("AzureOpenAi"));
builder.Services.Configure<ChatHistoryHandling>(builder.Configuration.GetSection("ChatHistoryHandling"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

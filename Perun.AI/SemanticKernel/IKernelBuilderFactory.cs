using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Perun.AI.Configuration;

namespace Perun.AI.SemanticKernel;

public interface IKernelBuilderFactory
{
    IKernelBuilder CreateKernelBuilderWithResiliency();
}

public class DefaultKernelBuilderFactory : IKernelBuilderFactory
{
    private readonly ILogger<DefaultKernelBuilderFactory> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IOptions<AzureOpenAiSettings> openAiConfig;

    public DefaultKernelBuilderFactory(ILogger<DefaultKernelBuilderFactory> logger, IHttpClientFactory httpClientFactory, IOptions<AzureOpenAiSettings> openAiConfig)
    {
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
        this.openAiConfig = openAiConfig;
    }

    public IKernelBuilder CreateKernelBuilderWithResiliency()
    {
        var model = openAiConfig.Value.Model;
        var endpoint = openAiConfig.Value.Endpoint;
        var apiKey = openAiConfig.Value.Key;

        var builder = Kernel.CreateBuilder();

        if (httpClientFactory != null)
        {
            HttpClient client = httpClientFactory.CreateClient("llm-http-client");
            if (client == null)
            {
                throw new NotSupportedException(
                    "HttpClientFactory failed to create a \"llm-http-client\" http client.");
            }

            builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey, httpClient: client);
        }
        else
        {
            builder.AddAzureOpenAIChatCompletion(model, endpoint, apiKey);
        }
        
        
        builder.Services.AddSingleton<ILogger>(sp => logger);
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddProvider(new SemanticKernelLoggerProvider(logger));
        });
        
        // todo: how to apply retry policy to kernel on all operations?
        // var kernelRetryPolicy = Policy
        //     .Handle<Exception>()
        //     .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        // builder.Services.AddSingleton(kernelRetryPolicy);
        
        return builder;
    }
    
    public class SemanticKernelLoggerProvider : ILoggerProvider
    {
        private readonly ILogger logger;

        public SemanticKernelLoggerProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return logger;
        }

        public void Dispose()
        {
        }
    }
    
 
  
}
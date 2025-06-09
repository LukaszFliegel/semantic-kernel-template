using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Perun.AI.Configuration;

public static class HttpClientBuilderExtensions
{
    public static IHttpStandardResiliencePipelineBuilder AddStandardResilienceHandlerWithDefaultsForLlmCalls(
        this IHttpClientBuilder builder)
    {
        return builder.AddStandardResilienceHandler(conf =>
        {
            conf.AttemptTimeout.Timeout = TimeSpan.FromSeconds(60);
            conf.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(180);
            conf.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(120);
        });
    }

    public static void AddLlmHttpClient(this IServiceCollection serviceCollection, bool useHttpRequestBodiesLogging = true)
    {
        var httpClientBuilder = serviceCollection.AddHttpClient("llm-http-client");
        if (useHttpRequestBodiesLogging)
        {
            serviceCollection.AddScoped<HttpLoggingHandler>();
            httpClientBuilder.AddHttpMessageHandler<HttpLoggingHandler>();
        }
        httpClientBuilder.AddStandardResilienceHandlerWithDefaultsForLlmCalls();
    }
    
}
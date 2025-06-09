using Microsoft.Extensions.Logging;

namespace Perun.AI.Configuration;

/// <summary>
/// low level logging of http requests and responses bodies for debugging purposes
/// </summary>
public class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> _logger;

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
    {
        _logger = logger;
    }

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger, HttpMessageHandler innerHandler) : base(innerHandler)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,  CancellationToken cancellationToken)
    {
        if (request.Content != null)
        {
            string requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug($"SENDING HTTP REQUEST BODY: {requestBody}");
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.Content != null)
        {
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug($"RECEIVED HTTP RESPONSE BODY: {responseBody}");
        }

        return response;
    }
}
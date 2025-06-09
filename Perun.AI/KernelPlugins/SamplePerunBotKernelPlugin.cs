using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Perun.AI.SemanticKernel;
using MethodTimer;
using Perun.AI.ChatHistory;

namespace Perun.AI.KernelPlugins;

public class SamplePerunBotKernelPlugin : IKernelPlugin
{
    private readonly IChatStore _chatStore;
    private readonly ILogger<SamplePerunBotKernelPlugin> _logger;

    public SamplePerunBotKernelPlugin(
        IChatStore chatStore,
        ILogger<SamplePerunBotKernelPlugin> logger)
    {
        _chatStore = chatStore;
        _logger = logger;
    }
    
    [Time]
    [KernelFunction("get_context_field1")]
    [Description(
        "Returns latest field 1")]
    [return: Description("Context field content")]
    public async Task<string> GetMyContextField(
        Kernel kernel,
        [Description("Mandatory unique and persistent identifier for the chat")]
        string chatId)
    {
        try
        {
            var chatContext = _chatStore.GetChatContext(chatId);
            if (chatContext == null)
            {
                throw new Exception(
                    $"Can't get field from cache for chatId {chatId} as chat context is not found.");
            }

            return chatContext.MyContextField;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when getting field from cache.");
            throw;
        }
    }    
}
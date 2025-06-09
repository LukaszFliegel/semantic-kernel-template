using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Perun.AI.ChatHistory;

public class ChatHistoryCleaner : IChatHistoryCleaner
{
    private readonly ILogger<ChatHistoryCleaner> logger;
    private readonly ILongChatHistoryHandler? longChatHistoryHandler;

    public ChatHistoryCleaner(ILogger<ChatHistoryCleaner> logger, ILongChatHistoryHandler? longChatHistoryHandler = null)
    {
        this.logger = logger;
        this.longChatHistoryHandler = longChatHistoryHandler;
    }

    public async Task Cleanup(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
    {
        CompactFunctionResults(chatHistory);

        RemoveEphemeralItems(chatHistory);

        if (longChatHistoryHandler != null)
        {
            await longChatHistoryHandler.HandleLongChatHistory(chatHistory);
        }
    }

    private static void CompactFunctionResults(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
    {
        foreach(var item in chatHistory.Where(h => h.Role == AuthorRole.Tool))
        {
            var compactionMessage = "[Result removed from chat history to save space]";
            
            var functionResult = item.Items.FirstOrDefault(i => i is FunctionResultContent) as FunctionResultContent;

            item.Items.Remove(functionResult);
            item.Items.Add(new FunctionResultContent(functionResult.FunctionName, functionResult.PluginName, functionResult.CallId, compactionMessage));
        }
    }

    private static void RemoveEphemeralItems(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
    {
        var itemsToRemove = new List<ChatMessageContent>();
        foreach (var item in chatHistory.Where(h => h.Metadata != null && h.Metadata.Any(i => i.Key?.ToLowerInvariant() == "ephemeral")))
        {
            itemsToRemove.Add(item);
        }
        
        foreach(var item in itemsToRemove)
        {
            chatHistory.Remove(item);
        }
    }
}
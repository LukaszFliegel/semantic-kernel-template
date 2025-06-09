using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Perun.AI.Configuration;

namespace Perun.AI.ChatHistory;

public class LongChatHistoryCutOffHandler : ILongChatHistoryHandler
{
    private readonly ChatHistoryHandling _chatHistoryHandling;

    public LongChatHistoryCutOffHandler(IOptions<ChatHistoryHandling> chatHistoryHandling)
    {
        _chatHistoryHandling = chatHistoryHandling.Value;
    }

    public Task HandleLongChatHistory(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
    {
        var userMessages = chatHistory
            .Where(m => m.Role == AuthorRole.User)
            .ToList();

        if (userMessages.Count > _chatHistoryHandling.CutOffAfterAmountOfUserMessages)
        {
            List<ChatMessageContent> lastMessagesToKeep = new();
            int numberOfUserMessagesAdded = 0;
            foreach (var message in chatHistory.Reverse())
            {
                // if number of messages to keep is reached, stop adding user messages unless it's a system message
                if (numberOfUserMessagesAdded >= _chatHistoryHandling.CutOffAfterAmountOfUserMessages
                    && message.Role != AuthorRole.System)
                {
                    continue;
                }

                lastMessagesToKeep.Add(message);

                if (message.Role == AuthorRole.User)
                {
                    numberOfUserMessagesAdded++;
                }
            }

            // restore order (it was reversed to iterate from the end and keep last messages)
            lastMessagesToKeep.Reverse();

            chatHistory.Clear();
            chatHistory.AddRange(lastMessagesToKeep);
        }

        return Task.CompletedTask;
    }
}

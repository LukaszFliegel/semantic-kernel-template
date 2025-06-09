using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Perun.AI.Configuration;
using System.Text;

namespace Perun.AI.ChatHistory;

public class LongChatHistorySummarizeHandler : ILongChatHistoryHandler
{
    private readonly ChatHistoryHandling _chatHistoryHandling;
    private readonly Kernel _kernel;

    public LongChatHistorySummarizeHandler(IOptions<ChatHistoryHandling> chatHistoryHandling, Kernel kernel)
    {
        _chatHistoryHandling = chatHistoryHandling.Value;
        _kernel = kernel;
    }

    public async Task HandleLongChatHistory(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
    {
        var userMessages = chatHistory.Where(m => m.Role == AuthorRole.User).ToList();

        if (userMessages.Count >= _chatHistoryHandling.CutOffAfterAmountOfUserMessages)
        {
            var nonSystemMessages = chatHistory.Where(m => m.Role != AuthorRole.System).ToList();
            var systemMessages = chatHistory.Where(m => m.Role == AuthorRole.System).ToList();

            if (nonSystemMessages.Count > _chatHistoryHandling.SummarizeAmountOfLastMessages)
            {
                var messagesToSummarize = nonSystemMessages
                    .Take(nonSystemMessages.Count - _chatHistoryHandling.SummarizeAmountOfLastMessages)
                    .ToList();

                var messagesToKeep = nonSystemMessages
                    .Skip(nonSystemMessages.Count - _chatHistoryHandling.SummarizeAmountOfLastMessages)
                    .ToList();

                if (messagesToSummarize.Any())
                {
                    var summary = await SummarizeMessages(messagesToSummarize);

                    chatHistory.Clear();
                    chatHistory.AddRange(systemMessages);
                    chatHistory.Add(new ChatMessageContent(AuthorRole.System, summary));
                    chatHistory.AddRange(messagesToKeep);
                }
            }
        }
    }

    private async Task<string> SummarizeMessages(List<ChatMessageContent> messages)
    {
        var conversationText = new StringBuilder();
        foreach (var message in messages)
        {
            conversationText.AppendLine($"{message.Role}: {message.Content}");
        }

        var summarizationPrompt = @$"
Summarize the following conversation, try to keep it as short as possible while preserving the main points:
```
{conversationText.ToString()}
```
Respond wit only summary, do not ask any questions and do not explain what you did. Respond with summarized conversation.
In the summary, keep the actors of conversation - the user and the assistant. User is asking questions and assistant provides answers.";

        var summary = await _kernel.InvokePromptAsync(summarizationPrompt);

        return summary.ToString();
    }
}
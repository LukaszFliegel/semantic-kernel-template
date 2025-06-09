namespace Perun.AI.ChatHistory;

public interface ILongChatHistoryHandler
{
    Task HandleLongChatHistory(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory);
}
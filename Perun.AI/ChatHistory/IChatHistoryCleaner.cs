namespace Perun.AI.ChatHistory;

public interface IChatHistoryCleaner
{
    Task Cleanup(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory);
}
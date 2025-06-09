using Microsoft.SemanticKernel;

namespace Perun.AI;

public interface IPerunChatService
{
    /*Task<*/string StartChat(string? welcomeMessage);
    /*Task<*/bool HasChat(string chatId);
    Task<ChatMessageContent> GetChatResponse(string chatId, string userMessage);
}
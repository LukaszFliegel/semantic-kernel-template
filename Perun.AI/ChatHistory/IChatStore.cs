namespace Perun.AI.ChatHistory;

public interface IChatStore
{
    /*Task<*/ChatContext? GetChatContext(string chatId);
    /*Task */void SetChatContext(string chatId, ChatContext chatContext);
    /*Task<*/bool HasChatContext(string chatId);
}
using Microsoft.Extensions.Caching.Memory;

namespace Perun.AI.ChatHistory;

public class MemoryCacheChatStore : IChatStore
{
    private readonly IMemoryCache memoryCache;

    public MemoryCacheChatStore(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public /*async Task<*/ChatContext? GetChatContext(string chatId)
    {
        if (memoryCache.TryGetValue(chatId, out var value))
        {
            return value as ChatContext;
        }

        return null;
    }
    
    public /*async Task<*/bool HasChatContext(string chatId)
    {
        return memoryCache.TryGetValue(chatId, out var _);
    }

    public /*async Task*/ void SetChatContext(string chatId, ChatContext chatContext)
    {
        memoryCache.Set(chatId, chatContext,
            new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(30) });
    }
}
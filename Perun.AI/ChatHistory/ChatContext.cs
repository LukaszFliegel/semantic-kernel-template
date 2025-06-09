namespace Perun.AI.ChatHistory;

public class ChatContext
{
    public ChatContext(string chatId)
    {
        ChatId = chatId;
    }

    public string ChatId { get; }
    public Microsoft.SemanticKernel.ChatCompletion.ChatHistory ChatHistory { get; set; }
    
    // add data to your context here
    public string? MyContextField { get; set; } // examples: user CV/user name/uploaded summary
    
    //public IDictionary<string, MySearchResult> MySearchResults { get;set; } = new Dictionary<string, MySearchResult>();
}
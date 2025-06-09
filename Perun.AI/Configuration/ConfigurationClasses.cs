namespace Perun.AI.Configuration;


public class AzureOpenAiSettings
{
    public string Model { get; set; }
    public string EmbeddingsModel { get; set; }
    public string Endpoint { get; set; }
    public string Key { get; set; }
}


public class ChatHistoryHandling
{
    public int CutOffAfterAmountOfUserMessages { get; set; }
    public int SummarizeAmountOfLastMessages { get; set; }
    public int SummarizeWhenNumberOfMessages { get; set; }
}


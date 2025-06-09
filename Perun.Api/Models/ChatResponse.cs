namespace VacanciesChatBot.WebApi.Models;

public class ChatResponse(string chatId, string message)
{
    public string ChatId { get; set; } = chatId;
    public string Message { get; set; } = message;
}
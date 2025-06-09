namespace VacanciesChatBot.WebApi.Models;

public class ChatRequest
{
    public string ChatId { get; set; } = String.Empty;
    public string? Message { get; set; }

    //public IFormFile? UserContextFile { get; set; }
}
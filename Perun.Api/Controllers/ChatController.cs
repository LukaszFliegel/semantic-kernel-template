using Microsoft.AspNetCore.Mvc;
using Perun.AI;
using VacanciesChatBot.WebApi.Models;

namespace VacanciesChatBot.WebApi.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IPerunChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IPerunChatService chatService,
        ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<ActionResult<ChatResponse>> StartChat()
    {
        string chatId;
        const string defaultWelcomeMessage =
            @"Hello,
              I'm Perun, AI assistant. How can I help you today?";

        string assistantMessage = defaultWelcomeMessage;
        try
        {
            chatId = _chatService.StartChat(assistantMessage);
            _logger.LogTrace($"Started chat: {chatId}");
        }
        catch (Exception ex)
        {
            throw new Exception("Error when starting chat.", ex);
        }

        try
        {
            if (assistantMessage == null)
            {
                var response = await _chatService.GetChatResponse(chatId, "Hi");
                assistantMessage = response.Content;
            }
            
            return Ok(new ChatResponse(chatId, assistantMessage));
        }
        catch (Exception ex)
        {
            return GetOkActionResultWithHumanReadableErrorMessage(chatId, ex);
        }
    }

    private ActionResult<ChatResponse> GetOkActionResultWithHumanReadableErrorMessage(string chatId, Exception ex)
    {
        _logger.LogError(ex, $"[ChatId: {chatId}] Error when processing chat request.");
        return Ok(new ChatResponse(chatId,
            "I'm sorry, I'm having trouble processing your request. Please try again later."));
    }

    [HttpPost("ask")]
    public async Task<ActionResult<ChatResponse>> Ask([FromForm] ChatRequest request)
    {        
        try
        {
            string contextField = null;
            // example usage of user context field treated as a file, currently commented out
            //if (request.UserContextFile != null)
            //{
            //    if (Path.GetExtension(request.UserContextFile.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            //    {
            //        using var memoryStream = new MemoryStream();
            //        await request.UserContextFile.CopyToAsync(memoryStream);
            //        memoryStream.Position = 0;
            //        contextField = await _documentTextExtractor.ExtractTextFromDocument(memoryStream);
            //    }
            //    else if (Path.GetExtension(request.UserContextFile.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
            //    {
            //        using var reader = new StreamReader(request.UserContextFile.OpenReadStream());
            //        contextField = await reader.ReadToEndAsync();
            //    }
            //    else
            //    {
            //        var responseProblem = await _chatService.ExplainProblem(request.ChatId,
            //            "Invalid file format. Only PDF and TXT files are supported.");
            //        return Ok(new ChatResponse(request.ChatId, responseProblem.Content));
            //    }
            //}

            var response = await _chatService.GetChatResponse(request.ChatId, /*contextField,*/ request.Message);

            return Ok(new ChatResponse(request.ChatId, response.Content));
        }
        catch (Exception ex)
        {
            return GetOkActionResultWithHumanReadableErrorMessage(request.ChatId, ex);
        }
    }
}
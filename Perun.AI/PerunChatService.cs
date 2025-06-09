using MethodTimer;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Perun.AI.ChatHistory;
using Polly;

namespace Perun.AI;

public class PerunChatService : IPerunChatService
{
    //private const long LlmSeed = 123456789;

    private string SystemPrompt =
        $@"You are a friendly and polite assistant ";

    private readonly Kernel kernel;
    private readonly IChatStore chatStore;
    private readonly IChatHistoryCleaner historyCleaner;
    private readonly ILogger<PerunChatService> logger;

    public PerunChatService(Kernel kernel, IChatStore chatStore, IChatHistoryCleaner historyCleaner,
        ILogger<PerunChatService> logger)
    {
        this.kernel = kernel;
        this.chatStore = chatStore;
        this.historyCleaner = historyCleaner;
        this.logger = logger;
    }

    public /*async Task<*/string StartChat(string? welcomeMessage)
    {
        var chatId = Guid.NewGuid().ToString();
        var chatContext = new ChatContext(chatId)
        {
            ChatHistory = StartChatHistory(chatId, welcomeMessage)
        };
        SaveChatContext(chatId, chatContext);

        return chatId;
    }

    public /*async Task<*/bool HasChat(string chatId)
    {
        return chatStore.HasChatContext(chatId);
    }

    [Time]
    public async Task<ChatMessageContent> GetChatResponse(string chatId, string? userMessage)
    {
        if (string.IsNullOrEmpty(chatId))
        {
            throw new ArgumentNullException(chatId);
        }

        logger.LogInformation($"[{chatId}][Chat] User says: [{userMessage}]");

        SetChatIdInKernelData(chatId);

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        var chatContext = GetChatContext(chatId);
        if (chatContext == null)
        {
            throw new InvalidOperationException($"Chat {chatId} not found. Start a new chat first.");
        }

        chatContext.ChatHistory.AddMessage(AuthorRole.User, userMessage ?? "");

        SaveChatContext(chatId, chatContext);

        // simple retry policy on any exception
        var result = await Policy
            .Handle<Exception>()
            .RetryAsync(2)
            .ExecuteAsync(async () => await chatCompletionService.GetChatMessageContentAsync(
                chatContext.ChatHistory,
                executionSettings: CreatePromptExecutionSettings(),
                kernel: kernel
            ));

        chatContext.ChatHistory.AddMessage(result.Role, result.Content);

        await historyCleaner.Cleanup(chatContext.ChatHistory);

        SaveChatContext(chatId, chatContext);

        logger.LogInformation($"[{chatId}][Chat] Bot says: [{result.Content}]");

        return result;
    }

    private /*async Task<*/ChatContext? GetChatContext(string chatId)
    {
        var chatContext = chatStore.GetChatContext(chatId);
        if (chatContext == null)
        {
            return null;
        }

        if (chatContext.ChatHistory == null || chatContext.ChatHistory.Count == 0)
        {
            chatContext.ChatHistory = StartChatHistory(chatId);
        }

        return chatContext;
    }

    private Microsoft.SemanticKernel.ChatCompletion.ChatHistory StartChatHistory(string chatId,
        string? assistantWelcomeMessage = null)
    {
        var chatHistory = new Microsoft.SemanticKernel.ChatCompletion.ChatHistory
        {
            new(AuthorRole.System, $"[chatId: {chatId}]"),
            new(AuthorRole.System, SystemPrompt),
        };
        if (!string.IsNullOrWhiteSpace(assistantWelcomeMessage))
        {
            chatHistory.AddMessage(AuthorRole.System, assistantWelcomeMessage);
        }

        return chatHistory;
    }

    private /*async Task*/ void SaveChatContext(string chatId, ChatContext chatContext)
    {
        chatStore.SetChatContext(chatId, chatContext);
    }

    protected virtual PromptExecutionSettings CreatePromptExecutionSettings()
    {
        return new AzureOpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options:
                new FunctionChoiceBehaviorOptions
                {
                    AllowParallelCalls = false,
                }),
            Temperature = 0.1,
            //Seed = LlmSeed,
        };
    }

    private void SetChatIdInKernelData(string chatId)
    {
        kernel.Data["chatId"] = chatId;
    }
}
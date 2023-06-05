using System.Diagnostics.CodeAnalysis;
using OpenAI_API;
using OpenAI_API.Chat;

namespace LangChain.Providers;

/// <summary>
/// https://openai.com/
/// </summary>
public class OpenAiModel : IChatModel, IPaidLargeLanguageModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string Model { get; init; } = OpenAI_API.Models.Model.ChatGPTTurbo;
    
    /// <inheritdoc/>
    public Usage TotalUsage { get; private set; }
    
    /// <inheritdoc/>
    public int ContextLength => OpenAiModelHelpers.CalculateContextLength(Model);

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    public OpenAiModel()
    {
    }
    
    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [SetsRequiredMembers]
    public OpenAiModel(string apiKey)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var api = new OpenAIAPI(apiKeys: ApiKey);
        var chat = api.Chat.CreateConversation();
        
        foreach (var message in request.Messages)
        {
            chat.AppendMessage(new ChatMessage(
                role: message.Role switch
                {
                    MessageRole.System => ChatMessageRole.System,
                    MessageRole.Ai => ChatMessageRole.Assistant,
                    MessageRole.Human => ChatMessageRole.User,
                    _ => ChatMessageRole.User,
                },
                content: message.Content));
        }
        
        var markdown = await chat.GetResponseFromChatbotAsync().ConfigureAwait(false);
        
        var completionTokens = chat.MostResentAPIResult.Usage.CompletionTokens;
        var promptTokens = chat.MostResentAPIResult.Usage.PromptTokens;
        var priceInUsd = CalculatePriceInUsd(
            completionTokens: completionTokens,
            promptTokens: promptTokens);
        
        var usage = new Usage(
            PromptTokens: promptTokens,
            CompletionTokens: completionTokens,
            TotalTokens: chat.MostResentAPIResult.Usage.TotalTokens,
            Messages: 1,
            PriceInUsd: priceInUsd);
        TotalUsage += usage;
            
        return new ChatResponse(
            Messages: new []{ new Message(markdown, MessageRole.Ai) },
            Usage: usage);
    }

    /// <inheritdoc/>
    public double CalculatePriceInUsd(int promptTokens, int completionTokens)
    {
        return OpenAiModelHelpers.CalculatePriceInUsd(
            modelId: Model,
            completionTokens: completionTokens,
            promptTokens: promptTokens);
    }
    
    #endregion
}
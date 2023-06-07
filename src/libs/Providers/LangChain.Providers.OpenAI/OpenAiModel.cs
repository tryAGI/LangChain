using System.Diagnostics.CodeAnalysis;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

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
    public string Id { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public string ApiKey { get; init; }
    
    /// <inheritdoc/>
    public Usage TotalUsage { get; private set; }
    
    /// <inheritdoc/>
    public int ContextLength => OpenAiModelHelpers.CalculateContextLength(Id);

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [SetsRequiredMembers]
    public OpenAiModel(string apiKey, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Id = id ?? throw new ArgumentNullException(nameof(id));
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
        chat.Model = new Model(Id)
        {
            OwnedBy = "openai"
        };
        
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
    public int CountTokens(string text)
    {
        return OpenAiModelHelpers.CountTokens(
            modelId: Id,
            text: text);
    }

    /// <inheritdoc/>
    public int CountTokens(IReadOnlyCollection<Message> messages)
    {
        return CountTokens(string.Join(
            Environment.NewLine,
            messages.Select(static x => x.Content)));
    }

    /// <inheritdoc/>
    public int CountTokens(ChatRequest request)
    {
        return CountTokens(request.Messages);
    }
    
    /// <inheritdoc/>
    public double CalculatePriceInUsd(int promptTokens, int completionTokens)
    {
        return OpenAiModelHelpers.CalculatePriceInUsd(
            modelId: Id,
            completionTokens: completionTokens,
            promptTokens: promptTokens);
    }
    
    #endregion
}
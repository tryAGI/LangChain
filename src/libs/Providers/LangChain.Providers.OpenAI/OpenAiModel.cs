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
    public int ContextLength => ApiHelpers.CalculateContextLength(Id);
    
    /// <summary>
    /// 
    /// </summary>
    public Tiktoken.Encoding Encoding { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OpenAiModel(string apiKey, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Id = id ?? throw new ArgumentNullException(nameof(id));
        
        Encoding = Tiktoken.Encoding.ForModel(Id);
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5),
        };
        var api = new OpenAiApi(apiKey: ApiKey, httpClient);
        var response = await api.CreateChatCompletionAsync(new CreateChatCompletionRequest
        {
            Messages = request.Messages
                .Select(static x => x.Role switch
                {
                    MessageRole.System => new ChatCompletionRequestMessage
                    {
                        Role = ChatCompletionRequestMessageRole.System,
                        Content = x.Content,
                    },
                    MessageRole.Ai => x.Content.AsAssistantMessage(),
                    MessageRole.Human => x.Content.AsUserMessage(),
                    _ => x.Content.AsUserMessage(),
                })
                .ToArray(),
            Model = Id,
        }, cancellationToken).ConfigureAwait(false);
        
        var message = response.GetFirstChoiceMessage();
        
        var completionTokens = response.Usage?.Completion_tokens ?? 0;
        var promptTokens = response.Usage?.Prompt_tokens ?? 0;
        var priceInUsd = CalculatePriceInUsd(
            completionTokens: completionTokens,
            promptTokens: promptTokens);
        
        var usage = new Usage(
            PromptTokens: promptTokens,
            CompletionTokens: completionTokens,
            Messages: 1,
            PriceInUsd: priceInUsd);
        TotalUsage += usage;
            
        return new ChatResponse(
            Messages: new []{ new Message(message.Content ?? string.Empty, MessageRole.Ai) },
            Usage: usage);
    }

    /// <inheritdoc/>
    public int CountTokens(string text)
    {
        return Encoding.CountTokens(text);
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
        return ApiHelpers.CalculatePriceInUsd(
            modelId: Id,
            completionTokens: completionTokens,
            promptTokens: promptTokens);
    }
    
    #endregion
}
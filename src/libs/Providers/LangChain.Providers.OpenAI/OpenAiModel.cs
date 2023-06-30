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
    
    private HttpClient HttpClient { get; set; }
    private Tiktoken.Encoding Encoding { get; set; }
    private ICollection<ChatCompletionFunctions> GlobalFunctions { get; set; } = new List<ChatCompletionFunctions>();

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OpenAiModel(string apiKey, HttpClient httpClient, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
        var api = new OpenAiApi(apiKey: ApiKey, HttpClient);
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
            Functions = GlobalFunctions,
            Function_call = Function_call4.Auto,
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
            Messages: request.Messages
                .Concat(new []{ new Message(message.Content ?? string.Empty, MessageRole.Ai) })
                .ToArray(),
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
    
    /// <summary>
    /// Adds user-defined OpenAI functions to each request to the model.
    /// </summary>
    /// <param name="functions"></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public void AddGlobalFunctions(ICollection<ChatCompletionFunctions> functions)
    {
        GlobalFunctions = functions ?? throw new ArgumentNullException(nameof(functions));
    }
    
    #endregion
}
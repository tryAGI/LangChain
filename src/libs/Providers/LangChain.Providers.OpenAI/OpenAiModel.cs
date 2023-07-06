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

    /// <summary>
    /// 
    /// </summary>
    public bool CallFunctionsAutomatically { get; set; } = true;
    
    /// <summary>
    /// 
    /// </summary>
    public bool ReplyToFunctionCallsAutomatically { get; set; } = true;
    
    /// <inheritdoc/>
    public Usage TotalUsage { get; private set; }
    
    /// <inheritdoc/>
    public int ContextLength => ApiHelpers.CalculateContextLength(Id);
    
    private HttpClient HttpClient { get; set; }
    private Tiktoken.Encoding Encoding { get; set; }
    private ICollection<ChatCompletionFunctions> GlobalFunctions { get; set; } = new List<ChatCompletionFunctions>();
    private IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> Calls { get; set; } = new Dictionary<string, Func<string, CancellationToken, Task<string>>>();

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

    private static ChatCompletionRequestMessage ToRequestMessage(Message message)
    {
        return new ChatCompletionRequestMessage
        {
            Role = message.Role switch
            {
                MessageRole.System => ChatCompletionRequestMessageRole.System,
                MessageRole.Ai => ChatCompletionRequestMessageRole.Assistant,
                MessageRole.FunctionCall => ChatCompletionRequestMessageRole.Assistant,
                MessageRole.Human => ChatCompletionRequestMessageRole.User,
                MessageRole.FunctionResult => ChatCompletionRequestMessageRole.Function,
                _ => ChatCompletionRequestMessageRole.User,
            },
            Name = string.IsNullOrWhiteSpace(message.FunctionName)
                ? null
                : message.FunctionName,
            Content = message.Content,
        };
    }
    
    private static Message ToMessage(ChatCompletionResponseMessage message)
    {
        return new Message(
            Content: message.Function_call?.Arguments ?? message.Content ?? string.Empty,
            Role: message.Role switch
            {
                ChatCompletionResponseMessageRole.System => MessageRole.System,
                ChatCompletionResponseMessageRole.User => MessageRole.Human,
                ChatCompletionResponseMessageRole.Assistant when message.Function_call != null => MessageRole.FunctionCall,
                ChatCompletionResponseMessageRole.Assistant => MessageRole.Ai,
                ChatCompletionResponseMessageRole.Function => MessageRole.FunctionResult,
                _ => MessageRole.Human,
            },
            FunctionName: message.Function_call?.Name);
    }
    
    private async Task<CreateChatCompletionResponse> CreateChatCompletionAsync(
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        var api = new OpenAiApi(apiKey: ApiKey, HttpClient);
        
        return await api.CreateChatCompletionAsync(new CreateChatCompletionRequest
        {
            Messages = messages
                .Select(ToRequestMessage)
                .ToArray(),
            Functions = GlobalFunctions,
            Function_call = Function_call4.Auto,
            Model = Id,
        }, cancellationToken).ConfigureAwait(false);
    }

    private Usage GetUsage(CreateChatCompletionResponse response)
    {
        var completionTokens = response.Usage?.Completion_tokens ?? 0;
        var promptTokens = response.Usage?.Prompt_tokens ?? 0;
        var priceInUsd = CalculatePriceInUsd(
            completionTokens: completionTokens,
            promptTokens: promptTokens);
        
        return new Usage(
            PromptTokens: promptTokens,
            CompletionTokens: completionTokens,
            Messages: 1,
            PriceInUsd: priceInUsd);
    }
    
    /// <inheritdoc/>
    public async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var messages = request.Messages.ToList();
        var response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);
        
        var message = response.GetFirstChoiceMessage();
        messages.Add(ToMessage(message));
        
        var usage = GetUsage(response);
        TotalUsage += usage;

        if (CallFunctionsAutomatically && message.Function_call != null)
        {
            var functionName = message.Function_call.Name ?? string.Empty;
            var func = Calls[functionName];
            var json = await func(message.Function_call.Arguments ?? string.Empty, cancellationToken).ConfigureAwait(false);
            messages.Add(json.AsFunctionResultMessage(functionName));

            if (ReplyToFunctionCallsAutomatically)
            {
                response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);
                messages.Add(ToMessage(response.GetFirstChoiceMessage()));
                usage += GetUsage(response);
                TotalUsage += usage;
            }
        }
        
        return new ChatResponse(
            Messages: messages,
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
    /// <param name="calls"></param>
    /// <returns></returns>
    [CLSCompliant(false)]
    public void AddGlobalFunctions(
        ICollection<ChatCompletionFunctions> functions,
        IReadOnlyDictionary<string, Func<string, CancellationToken, Task<string>>> calls)
    {
        GlobalFunctions = functions ?? throw new ArgumentNullException(nameof(functions));
        Calls = calls ?? throw new ArgumentNullException(nameof(calls));
    }
    
    #endregion
}
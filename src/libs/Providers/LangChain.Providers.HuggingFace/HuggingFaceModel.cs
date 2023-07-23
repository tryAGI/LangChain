namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class HuggingFaceModel : IChatModel
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

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HuggingFaceModel(string apiKey, HttpClient httpClient, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Id = id ?? throw new ArgumentNullException(nameof(id));
        
        Encoding = Tiktoken.Encoding.ForModel(Id);
    }

    #endregion

    #region Methods

    private static string ToRequestMessage(Message message)
    {
        return message.Role switch
        {
            MessageRole.System => message.Content.AsAssistantMessage(),
            MessageRole.Ai => message.Content.AsAssistantMessage(),
            MessageRole.Human => StringExtensions.AsHumanMessage(message.Content),
            MessageRole.FunctionCall => throw new NotImplementedException(),
            MessageRole.FunctionResult => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }
    
    private static Message ToMessage(ICollection<GenerateTextResponseValue> message)
    {
        return new Message(
            Content: message.First().Generated_text,
            Role: MessageRole.Ai);
    }
    
    private async Task<ICollection<GenerateTextResponseValue>> CreateChatCompletionAsync(
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        var api = new HuggingFaceApi(apiKey: ApiKey, HttpClient);
        
        return await api.GenerateTextAsync(modelId: Id, body: new GenerateTextRequest
        {
            Inputs = messages
                .Select(ToRequestMessage)
                .ToArray().AsPrompt(),
            Parameters = new GenerateTextRequestParameters
            {
                Max_new_tokens = 250,
            },
            Options = new GenerateTextRequestOptions
            {
                Use_cache = true,
                Wait_for_model = false,
            },
        }, cancellationToken).ConfigureAwait(false);
    }

    private Usage GetUsage(IReadOnlyCollection<Message> messages)
    {
        var completionTokens = CountTokens(messages.Last().Content);
        var promptTokens = CountTokens(messages
            .Take(messages.Count - 1)
            .Select(ToRequestMessage)
            .ToArray()
            .AsPrompt());
        
        return new Usage(
            PromptTokens: promptTokens,
            CompletionTokens: completionTokens,
            Messages: 1,
            PriceInUsd: 0.0);
    }
    
    /// <inheritdoc/>
    public async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var messages = request.Messages.ToList();
        var response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);
        
        messages.Add(ToMessage(response));
        
        var usage = GetUsage(messages);
        TotalUsage += usage;
        
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
    
    #endregion
}
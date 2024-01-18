using System.Diagnostics;

namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class OllamaLanguageModelInstruction : IChatModel
{
    private readonly string _modelName;
    private readonly OllamaApiClient _api;
    
    /// <summary>
    /// 
    /// </summary>
    public OllamaLanguageModelOptions Options { get; }
    
    /// <inheritdoc />
    public string Id => "ollama";
    
    /// <inheritdoc />
    public Usage TotalUsage { get; set; }
    
    /// <inheritdoc />
    public int ContextLength { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool UseJson { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="url"></param>
    /// <param name="options"></param>
    public OllamaLanguageModelInstruction(
        string modelName,
        string url = "http://localhost:11434",
        OllamaLanguageModelOptions? options = null)
    {
        _modelName = modelName;
        Options = options ?? new OllamaLanguageModelOptions();
        _api = new OllamaApiClient(url);
        
    }
    /// <summary>
    /// Occurs when token generated.
    /// </summary>
    public event Action<string> TokenGenerated = delegate { };

    /// <summary>
    /// Occurs before prompt is sent to the model.
    /// </summary>
    public event Action<string> PromptSent = delegate { };

    /// <inheritdoc />
    public async Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var models = await _api.ListLocalModels().ConfigureAwait(false);
        
        if (models.All(x => x.Name != _modelName))
        {
            await _api.PullModel(_modelName).ConfigureAwait(false);
        }
        var prompt = ToPrompt(request.Messages);

        var watch = Stopwatch.StartNew();
        var response = _api.GenerateCompletion(new GenerateCompletionRequest()
        {
            Prompt = prompt,
            Model = _modelName,
            Options = Options,
            Stream = true,
            Raw = true,
            Format = UseJson ? "json" : string.Empty,
        });
        PromptSent(prompt);
        var buf = "";
        await foreach (var completion in response)
        {
            buf += completion.Response;
            TokenGenerated(completion.Response);
        }

        var result = request.Messages.ToList();
        result.Add(buf.AsAiMessage());

        watch.Stop();

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        TotalUsage += usage;

        return new ChatResponse(
            Messages: result,
            Usage: usage);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    protected static string ConvertRole(MessageRole role)
    {
        return role switch
        {
            MessageRole.Human => "Human: ",
            MessageRole.Ai => "Assistant: ",
            MessageRole.System => "",
            _ => throw new NotSupportedException($"the role {role} is not supported")
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected static string ConvertMessage(Message message)
    {
        return $"{ConvertRole(message.Role)}{message.Content}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    protected static string ToPrompt(IEnumerable<Message> messages)
    {
        return string.Join("\n", messages.Select(ConvertMessage).ToArray());
    }

}
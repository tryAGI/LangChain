using System.Diagnostics;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class OllamaChatModel(
    OllamaProvider provider,
    string id)
    : ChatModel(id), IChatModel
{
    /// <summary>
    /// Provider of the model.
    /// </summary>
    public OllamaProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc />
    public override int ContextLength => 0;

    /// <summary>
    /// 
    /// </summary>
    public bool UseJson { get; set; }

    /// <inheritdoc />
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var models = await Provider.Api.ListLocalModels().ConfigureAwait(false);
        if (models.All(x => x.Name != Id))
        {
            await Provider.Api.PullModel(Id).ConfigureAwait(false);
        }

        var prompt = ToPrompt(request.Messages);
        var watch = Stopwatch.StartNew();
        var response = Provider.Api.GenerateCompletion(new GenerateCompletionRequest()
        {
            Prompt = prompt,
            Model = Id,
            Options = Provider.Options,
            Stream = true,
            Raw = true,
            Format = UseJson ? "json" : string.Empty,
        });

        OnPromptSent(prompt);

        var buf = "";
        await foreach (var completion in response)
        {
            buf += completion.Response;
            OnPartialResponseGenerated(completion.Response);
        }

        OnCompletedResponseGenerated(buf);

        var result = request.Messages.ToList();
        result.Add(buf.AsAiMessage());

        watch.Stop();

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = result,
            Usage = usage,
            UsedSettings = ChatSettings.Default,
        };
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
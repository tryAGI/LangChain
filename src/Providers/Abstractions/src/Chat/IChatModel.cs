namespace LangChain.Providers;

/// <summary>
/// Defines a large language model that can be used for chat.
/// </summary>
public interface IChatModel : IModel<ChatSettings>
{
    /// <summary>
    /// Max input tokens for the model.
    /// </summary>
    public int ContextLength { get; }


    /// <summary>
    /// Occurs when token generated in streaming mode.
    /// </summary>
    event EventHandler<string>? PartialResponseGenerated;

    /// <summary>
    /// Occurs when completed response generated.
    /// </summary>
    event EventHandler<string>? CompletedResponseGenerated;

    /// <summary>
    /// Occurs before prompt is sent to the model.
    /// </summary>
    event EventHandler<string>? PromptSent;


    /// <summary>
    /// Run the LLM on the given prompt and input.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default);
}
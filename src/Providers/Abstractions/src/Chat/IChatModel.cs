namespace LangChain.Providers;

/// <summary>
/// Defines a large language model that can be used for chat.
/// </summary>
public interface IChatModel : IModel<ChatSettings>
{
    /// <summary>
    /// Gets the maximum number of input tokens the model can handle.
    /// This property defines the upper limit of tokens that can be processed in a single request.
    /// Implementations should ensure that requests do not exceed this limit.
    /// </summary>
    public int ContextLength { get; }

    /// <summary>
    /// Occurs when a partial response is generated.
    /// </summary>
    event EventHandler<string>? PartialResponseGenerated;

    /// <summary>
    /// Occurs when a completed response is generated.
    /// </summary>
    event EventHandler<string>? CompletedResponseGenerated;

    /// <summary>
    /// Occurs when a prompt is sent to the chat model.
    /// </summary>
    event EventHandler<string>? PromptSent;


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
    /// Asynchronously generates a chat response based on the provided request and settings.
    /// </summary>
    /// <param name="request">The chat request containing the prompt and any additional information required for generating a response.</param>
    /// <param name="settings">Optional chat settings to customize the response generation process. If null, default settings are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the chat response.</returns>
    public Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default);
}
// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Represents an abstract base class for chat models, providing common functionality and event handling.
/// </summary>
/// <param name="id">The unique identifier for the chat model.</param>
public abstract class ChatModel(string id) : Model<ChatSettings>(id), IChatModel<ChatRequest, ChatResponse, ChatSettings>
{
    #region Events

    /// <summary>
    /// Gets or sets the context length for the chat model.
    /// </summary>
    public virtual int ContextLength { get; protected set; }

    /// <summary>
    /// Occurs when a partial response is generated.
    /// </summary>
    public event EventHandler<string>? PartialResponseGenerated;
    
    protected void OnPartialResponseGenerated(string token)
    {
        PartialResponseGenerated?.Invoke(this, token);
    }

    /// <summary>
    /// Occurs when a completed response is generated.
    /// </summary>
    public event EventHandler<string>? CompletedResponseGenerated;
    
    protected void OnCompletedResponseGenerated(string token)
    {
        CompletedResponseGenerated?.Invoke(this, token);
    }
    
    /// <summary>
    /// Occurs when a prompt is sent to the chat model.
    /// </summary>
    public event EventHandler<string>? PromptSent;
    

    protected void OnPromptSent(string prompt)
    {
        PromptSent?.Invoke(this, prompt);
    }

    #endregion

    /// <summary>
    /// Generates a chat response asynchronously based on the provided request and settings.
    /// </summary>
    /// <param name="request">The chat request.</param>
    /// <param name="settings">Optional chat settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, containing the chat response.</returns>
    public abstract Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default);
}
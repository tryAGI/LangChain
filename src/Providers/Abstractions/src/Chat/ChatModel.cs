// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public abstract class ChatModel(string id) : Model<ChatSettings>(id), IChatModel<ChatRequest, ChatResponse, ChatSettings>
{
    #region Events

    public virtual int ContextLength { get; protected set; }

    /// <inheritdoc cref="IChatModel.PartialResponseGenerated"/>
    public event EventHandler<string>? PartialResponseGenerated;

    protected void OnPartialResponseGenerated(string token)
    {
        PartialResponseGenerated?.Invoke(this, token);
    }

    /// <inheritdoc cref="IChatModel.CompletedResponseGenerated"/>
    public event EventHandler<string>? CompletedResponseGenerated;

    protected void OnCompletedResponseGenerated(string token)
    {
        CompletedResponseGenerated?.Invoke(this, token);
    }

    /// <inheritdoc cref="IChatModel.PromptSent"/>
    public event EventHandler<string>? PromptSent;


    protected void OnPromptSent(string prompt)
    {
        PromptSent?.Invoke(this, prompt);
    }

    #endregion

    public abstract Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default);
}
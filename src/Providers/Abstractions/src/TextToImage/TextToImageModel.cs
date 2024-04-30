namespace LangChain.Providers;

public abstract class TextToImageModel(string id) : Model<TextToImageSettings>(id)
{
    #region Events

    /// <inheritdoc cref="IChatModel.PromptSent"/>
    public event EventHandler<string>? PromptSent;

    protected void OnPromptSent(string prompt)
    {
        PromptSent?.Invoke(this, prompt);
    }

    #endregion
}
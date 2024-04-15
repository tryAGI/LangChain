namespace LangChain.Providers;

public abstract class TextToMusicModel(string id) : Model<TextToMusicSettings>(id)
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
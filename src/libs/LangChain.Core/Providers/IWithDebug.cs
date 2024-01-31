namespace LangChain.Providers;

public interface IWithDebug
{
    /// <summary>
    /// Occurs when token generated.
    /// </summary>
    public event Action<string> TokenGenerated;

    /// <summary>
    /// Occurs before prompt is sent to the model.
    /// </summary>
    public event Action<string> PromptSent;
}
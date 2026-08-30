namespace LangChain.Providers;

/// <summary>
/// Interface for models that support counting tokens.
/// </summary>
public interface ISupportsCountTokens
{
    /// <summary>
    /// Counts the number of tokens in the given text.
    /// </summary>
    public int CountTokens(string text);
}

namespace LangChain.Providers;

/// <summary>
/// Defines a large language model that supports counting tokens.
/// </summary>
public interface ISupportsCountTokens
{
    /// <summary>
    /// Counts the number of tokens in the given prompt and input.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int CountTokens(string text);

    /// <summary>
    /// Counts the number of tokens in the given prompt and input.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public int CountTokens(IReadOnlyCollection<Message> messages);

    /// <summary>
    /// Counts the number of tokens in the given prompt and input.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public int CountTokens(ChatRequest request);
}
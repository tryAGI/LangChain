namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface ISupportsCountTokens
{
    /// <summary>
    /// 
    /// </summary>
    public int ContextLength { get; }
    
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
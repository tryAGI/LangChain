namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface IChatModel
{
    /// <summary>
    /// 
    /// </summary>
    public Usage TotalUsage { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public int ContextLength { get; }

    /// <summary>
    /// Run the LLM on the given prompt and input.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of tokens in the given prompt and input.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int CountTokens(string text);

    /// <summary>
    /// Counts the number of tokens in the given prompt and input.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public int CountTokens(ChatRequest request);
}
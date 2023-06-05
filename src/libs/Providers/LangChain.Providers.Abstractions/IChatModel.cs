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
}
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface ISupportsModeration
{
    /// <summary>
    /// 
    /// </summary>
    public int RecommendedModerationChunkSize { get; }

    /// <summary>
    /// Given a input text, returns true if the model classifies it as violating provider content policy.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> CheckViolationAsync(string text, CancellationToken cancellationToken = default);
}
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface IModerationModel : IModel
{
    /// <summary>
    /// 
    /// </summary>
    public int RecommendedModerationChunkSize { get; }

    /// <summary>
    /// Given a input text, returns true if the model classifies it as violating provider content policy.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ModerationResponse> CheckViolationAsync(
        ModerationRequest request,
        ModerationSettings? settings = null,
        CancellationToken cancellationToken = default);
}
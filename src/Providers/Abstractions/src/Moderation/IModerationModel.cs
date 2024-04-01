namespace LangChain.Providers;

/// <summary>
/// Defines the interface for models that perform content moderation. Implementations of this interface should provide mechanisms to assess if given input text violates content policies.
/// </summary>
public interface IModerationModel : IModel
{
    /// <summary>
    /// Gets the recommended size of text chunks for moderation checks. This size is optimal for the model to accurately assess content for policy violations.
    /// </summary>
    public int RecommendedModerationChunkSize { get; }

    /// <summary>
    /// Asynchronously checks if the given input text violates content policies.
    /// </summary>
    /// <param name="request">The moderation request containing the text to be checked.</param>
    /// <param name="settings">Optional moderation settings to customize the check process. If null, default settings are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in a ModerationResponse indicating if a violation was found.</returns>
    public Task<ModerationResponse> CheckViolationAsync(
        ModerationRequest request,
        ModerationSettings? settings = null,
        CancellationToken cancellationToken = default);
}
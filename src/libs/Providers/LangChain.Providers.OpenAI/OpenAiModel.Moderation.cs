using OpenAI.Constants;
using OpenAI.Moderations;

namespace LangChain.Providers.OpenAI;

// ReSharper disable MemberCanBePrivate.Global

public partial class OpenAiModel : ISupportsModeration
{
    #region Properties

    /// <inheritdoc cref="OpenAiConfiguration.ModerationModelId"/>
    public string ModerationModelId { get; init; } = ModerationModel.Latest;

    /// <inheritdoc/>
    public int RecommendedModerationChunkSize => 2_000;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public async Task<bool> CheckViolationAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await Api.ModerationsEndpoint.CreateModerationAsync(
            new ModerationsRequest(
                input: text,
                model: ModerationModelId),
            cancellationToken).ConfigureAwait(false);

        return response.Results[0].Flagged;
    }

    #endregion
}
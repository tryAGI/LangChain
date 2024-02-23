using OpenAI.Moderations;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public class OpenAiModerationModel(
    OpenAiProvider provider,
    string id)
    : Model(id), IModerationModel
{
    /// <inheritdoc/>
    public int RecommendedModerationChunkSize => 2_000;

    /// <inheritdoc/>
    public async Task<ModerationResponse> CheckViolationAsync(
        ModerationRequest request,
        ModerationSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var response = await provider.Api.ModerationsEndpoint.CreateModerationAsync(
            new ModerationsRequest(
                input: request.Prompt,
                model: Id),
            cancellationToken).ConfigureAwait(false);

        return new ModerationResponse
        {
            IsValid = response.Results[0].Flagged,
            Usage = Usage.Empty,
        };
    }
}
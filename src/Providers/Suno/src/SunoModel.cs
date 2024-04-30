using LangChain.Providers.Suno.Sdk;

namespace LangChain.Providers.Suno;

public class SunoModel(
    SunoProvider provider,
    string id)
    : TextToMusicModel(id), ITextToMusicModel
{
    #region Methods

    /// <inheritdoc/>
    public async Task<TextToMusicResponse> GenerateMusicAsync(
        TextToMusicRequest request,
        TextToMusicSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        OnPromptSent(request.Prompt);

        var createResponse = await provider.HttpClient.GenerateV2Async(
            request: new GenerateV2Request
            {
                GptDescriptionPrompt = string.Empty,
                Mv = Id,
                Prompt = request.Prompt,
                MakeInstrumental = false,
                Tags = string.Empty,
                Title = string.Empty,
                ContinueClipId = string.Empty,
                ContinueAt = 0,
            },
            cancellationToken).ConfigureAwait(false);

        var completedClips = createResponse.Clips
            .Where(x => x.Status == "complete")
            .ToList();
        var nonCompletedClips = createResponse.Clips
            .Where(x => x.Status != "complete")
            .ToList();
        while (!cancellationToken.IsCancellationRequested && nonCompletedClips.Count > 0)
        {
            var clips = await provider.HttpClient.GetFeedAsync(
                ids: nonCompletedClips.Select(x => x.Id).ToArray(),
                cancellationToken).ConfigureAwait(false);

            nonCompletedClips = clips
                .Where(x => x.Status != "complete")
                .ToList();
            completedClips.AddRange(clips
                .Where(x => x.Status == "complete")
                .ToList());

            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken).ConfigureAwait(false);
        }

        return new TextToMusicResponse
        {
            Images = await Task.WhenAll(completedClips
                .Where(x => !string.IsNullOrWhiteSpace(x.AudioUrl))
                .Select(async x =>
                {
                    var bytes = await provider.HttpClient.GetByteArrayAsync(new Uri(x.AudioUrl), cancellationToken).ConfigureAwait(false);

                    return Data.FromBytes(bytes);
                })).ConfigureAwait(false),
            Usage = Usage.Empty,
            UsedSettings = settings ?? TextToMusicSettings.Default,
        };
    }

    #endregion
}
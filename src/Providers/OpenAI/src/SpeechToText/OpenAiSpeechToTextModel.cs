using OpenAI.Audio;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

/// <summary>
/// Wrapper around OpenAI large language models.
/// </summary>
/// <param name="provider"></param>
/// <param name="id"></param>
/// <exception cref="ArgumentNullException"></exception>
public class OpenAiSpeechToTextModel(
    OpenAiProvider provider,
    string id)
    : Model<SpeechToTextSettings>(id), ISpeechToTextModel
{
    /// <inheritdoc/>
    [CLSCompliant(false)]
    public async Task<SpeechToTextResponse> TranscribeAsync(
        SpeechToTextRequest request,
        SpeechToTextSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        try
        {
            var usedSettings = OpenAiSpeechToTextSettings.Calculate(
                requestSettings: settings,
                modelSettings: Settings,
                providerSettings: provider.SpeechToTextSettings);
            var response = await provider.Api.AudioEndpoint.CreateTranscriptionAsync(
#pragma warning disable CA2000 // User should dispose stream
                request: new AudioTranscriptionRequest(
                    audio: request.Stream,
                    model: usedSettings.Model!,
                    audioName: usedSettings.AudioName!,
                    prompt: usedSettings.Prompt!,
                    responseFormat: usedSettings.ResponseFormat!.Value,
                    temperature: usedSettings.Temperature,
                    language: usedSettings.Language!),
#pragma warning restore CA2000
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var usage = Usage.Empty with
            {
                //PriceInUsd = usedSettings.Model.Value.GetPriceInUsd(seconds: response.Seconds),
            };
            AddUsage(usage);
            provider.AddUsage(usage);

            return new SpeechToTextResponse
            {
                Text = response,
                Usage = usage,
            };
        }
        finally
        {
            if (request.OwnsStream)
            {
#if NET6_0_OR_GREATER
                await request.Stream.DisposeAsync().ConfigureAwait(false);
#else
                request.Stream.Dispose();
#endif
            }
        }
    }
}
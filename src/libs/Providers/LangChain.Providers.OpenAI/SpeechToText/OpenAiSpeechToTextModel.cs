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
            var usedSettings = new OpenAiSpeechToTextSettings
            {
                Model =
                    (settings as OpenAiSpeechToTextSettings)?.Model ??
                    (Settings as OpenAiSpeechToTextSettings)?.Model ??
                    (provider.SpeechToTextSettings as OpenAiSpeechToTextSettings)?.Model ??
                    OpenAiSpeechToTextSettings.Default.Model ??
                    throw new InvalidOperationException("Default Model is not set."),
                AudioName = 
                    (settings as OpenAiSpeechToTextSettings)?.AudioName ??
                    (Settings as OpenAiSpeechToTextSettings)?.AudioName ??
                    (provider.SpeechToTextSettings as OpenAiSpeechToTextSettings)?.AudioName ??
                    OpenAiSpeechToTextSettings.Default.AudioName ??
                    throw new InvalidOperationException("Default AudioName is not set."),
                Prompt =
                    (settings as OpenAiSpeechToTextSettings)?.Prompt ??
                    (Settings as OpenAiSpeechToTextSettings)?.Prompt ??
                    (provider.SpeechToTextSettings as OpenAiSpeechToTextSettings)?.Prompt ??
                    OpenAiSpeechToTextSettings.Default.Prompt ??
                    throw new InvalidOperationException("Default Prompt is not set."),
                ResponseFormat =
                    (settings as OpenAiSpeechToTextSettings)?.ResponseFormat ??
                    (Settings as OpenAiSpeechToTextSettings)?.ResponseFormat ??
                    (provider.SpeechToTextSettings as OpenAiSpeechToTextSettings)?.ResponseFormat ??
                    OpenAiSpeechToTextSettings.Default.ResponseFormat ??
                    throw new InvalidOperationException("Default ResponseFormat is not set."),
                Temperature =
                    (settings as OpenAiSpeechToTextSettings)?.Temperature ??
                    (Settings as OpenAiSpeechToTextSettings)?.Temperature ??
                    (provider.SpeechToTextSettings as OpenAiSpeechToTextSettings)?.Temperature ??
                    OpenAiSpeechToTextSettings.Default.Temperature ??
                    throw new InvalidOperationException("Default Temperature is not set."),
                Language =
                    (settings as OpenAiSpeechToTextSettings)?.Language ??
                    (Settings as OpenAiSpeechToTextSettings)?.Language ??
                    (provider.SpeechToTextSettings as OpenAiSpeechToTextSettings)?.Language ??
                    OpenAiSpeechToTextSettings.Default.Language ??
                    throw new InvalidOperationException("Default Language is not set."),
            };
            var response = await provider.Api.AudioEndpoint.CreateTranscriptionAsync(
#pragma warning disable CA2000 // User should dispose stream
                request: new AudioTranscriptionRequest(
                    audio: request.Stream,
                    model: usedSettings.Model,
                    audioName: usedSettings.AudioName,
                    prompt: usedSettings.Prompt,
                    responseFormat: usedSettings.ResponseFormat.Value,
                    temperature: usedSettings.Temperature,
                    language: usedSettings.Language),
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
using OpenAI.Audio;
using OpenAI.Models;

namespace LangChain.Providers.OpenAI;

public partial class OpenAiModel : ISpeechToTextModel<OpenAiSpeechToTextSettings?>
{
    #region Methods

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public async Task<string> TranscribeAsync(
        Stream stream,
        OpenAiSpeechToTextSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        stream = stream ?? throw new ArgumentNullException(nameof(stream));
        settings ??= OpenAiSpeechToTextSettings.Default;
        
        var response = await Api.AudioEndpoint.CreateTranscriptionAsync(
#pragma warning disable CA2000 // User should dispose stream
            request: new AudioTranscriptionRequest(
                audio: stream,
                model: new Model(settings.Value.Model),
                audioName: settings.Value.AudioName,
                prompt: settings.Value.Prompt,
                responseFormat: settings.Value.ResponseFormat,
                temperature: settings.Value.Temperature,
                language: settings.Value.Language),
#pragma warning restore CA2000
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            //PriceInUsd = settings.Value.Model.GetPriceInUsd(seconds: response.Seconds),
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return response;
    }

    #endregion
}
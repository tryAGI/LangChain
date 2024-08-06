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
    [CLSCompliant(false)]
    public OpenAiSpeechToTextModel(
        OpenAiProvider provider,
        CreateTranscriptionRequestModel id)
        : this(provider, id.ToValueString())
    {
    }

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
            using var memoryStream = new MemoryStream();
            await request.Stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            memoryStream.Position = 0;

            var response = await provider.Api.Audio.CreateTranscriptionAsync(
                file: memoryStream.ToArray(),
                filename: request.Filename ?? "file.wav",
                //audioName: usedSettings.AudioName!,
                model: usedSettings.Model ?? CreateTranscriptionRequestModel.Whisper1,
                prompt: usedSettings.Prompt!,
                responseFormat: usedSettings.ResponseFormat,
                temperature: usedSettings.Temperature!.Value,
                language: usedSettings.Language!,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var usage = Usage.Empty with
            {
                //PriceInUsd = usedSettings.Model.Value.GetPriceInUsd(seconds: response.Seconds),
            };
            AddUsage(usage);
            provider.AddUsage(usage);

            return new SpeechToTextResponse
            {
                Text = response.Value1?.Text ?? string.Empty,
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
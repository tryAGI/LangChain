using OpenAI.Audio;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

/// <summary>
/// 
/// </summary>
/// <param name="provider"></param>
/// <param name="id"></param>
public class OpenAiTextToSpeechModel(
    OpenAiProvider provider,
    string id)
    : Model<TextToSpeechSettings>(id), ITextToSpeechModel
{
    /// <inheritdoc/>
    public async Task<TextToSpeechResponse> GenerateSpeechAsync(
        TextToSpeechRequest request,
        TextToSpeechSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var usedSettings = new OpenAiTextToSpeechSettings
        {
            Model =
                (settings as OpenAiTextToSpeechSettings)?.Model ??
                (Settings as OpenAiTextToSpeechSettings)?.Model ??
                (provider.TextToSpeechSettings as OpenAiTextToSpeechSettings)?.Model ??
                OpenAiTextToSpeechSettings.Default.Model ??
                throw new InvalidOperationException("Default Model is not set."),
            Voice = 
                (settings as OpenAiTextToSpeechSettings)?.Voice ??
                (Settings as OpenAiTextToSpeechSettings)?.Voice ??
                (provider.TextToSpeechSettings as OpenAiTextToSpeechSettings)?.Voice ??
                OpenAiTextToSpeechSettings.Default.Voice ??
                throw new InvalidOperationException("Default Voice is not set."),
            ResponseFormat =
                (settings as OpenAiTextToSpeechSettings)?.ResponseFormat ??
                (Settings as OpenAiTextToSpeechSettings)?.ResponseFormat ??
                (provider.TextToSpeechSettings as OpenAiTextToSpeechSettings)?.ResponseFormat ??
                OpenAiTextToSpeechSettings.Default.ResponseFormat ??
                throw new InvalidOperationException("Default ResponseFormat is not set."),
            Speed =
                (settings as OpenAiTextToSpeechSettings)?.Speed ??
                (Settings as OpenAiTextToSpeechSettings)?.Speed ??
                (provider.TextToSpeechSettings as OpenAiTextToSpeechSettings)?.Speed ??
                OpenAiTextToSpeechSettings.Default.Speed ??
                throw new InvalidOperationException("Default Speed is not set."),
        };
        var response = await provider.Api.AudioEndpoint.CreateSpeechAsync(
            request: new SpeechRequest(
                input: request.Prompt,
                model: new global::OpenAI.Models.Model(usedSettings.Model),
                voice: usedSettings.Voice.Value,
                responseFormat: usedSettings.ResponseFormat.Value,
                speed: usedSettings.Speed),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = usedSettings.Model.Value.GetPriceInUsd(characters: request.Prompt.Length),
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new TextToSpeechResponse
        {
            Bytes = response.ToArray(),
            Usage = usage,
        };
    }
}
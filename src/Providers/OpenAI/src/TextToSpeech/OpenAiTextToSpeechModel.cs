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

        var usedSettings = OpenAiTextToSpeechSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.TextToSpeechSettings);
        var response = await provider.Api.AudioEndpoint.CreateSpeechAsync(
            request: new SpeechRequest(
                input: request.Prompt,
                model: new global::OpenAI.Models.Model(usedSettings.Model!),
                voice: usedSettings.Voice!.Value,
                responseFormat: usedSettings.ResponseFormat!.Value,
                speed: usedSettings.Speed),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = usedSettings.Model!.Value.GetPriceInUsd(characters: request.Prompt.Length),
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
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
    [CLSCompliant(false)]
    public OpenAiTextToSpeechModel(
        OpenAiProvider provider,
        CreateSpeechRequestModel id)
        : this(provider, id.ToValueString())
    {
    }
    
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
        var response = await provider.Api.Audio.CreateSpeechAsync(
            input: request.Prompt,
            model: usedSettings.Model ?? CreateSpeechRequestModel.Tts1,
            voice: usedSettings.Voice!.Value,
            responseFormat: usedSettings.ResponseFormat!.Value,
            speed: usedSettings.Speed ?? 1.0,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = usedSettings.Model?.Value2.GetPriceInUsd(characters: request.Prompt.Length) ?? double.NaN,
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
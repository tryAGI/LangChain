using StableDiffusion;

namespace LangChain.Providers.Automatic1111;

/// <summary>
/// 
/// </summary>
public class Automatic1111Model(
    string url = "http://localhost:7860/",
    HttpClient? httpClient = null)
    : TextToImageModel(id: "Automatic1111"), ITextToImageModel
{
    private readonly StableDiffusionClient _client = new(url, httpClient ?? new HttpClient());

    /// <inheritdoc />
    public async Task<TextToImageResponse> GenerateImageAsync(
        TextToImageRequest request,
        TextToImageSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        OnPromptSent(request.Prompt);

        var usedSettings = Automatic1111ModelSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: null);
        var samplers = await _client.Get_samplers_sdapi_v1_samplers_getAsync(
            cancellationToken).ConfigureAwait(false);
        var response = await _client.Text2imgapi_sdapi_v1_txt2img_postAsync(
            new StableDiffusionProcessingTxt2Img
            {
                Prompt = request.Prompt,
                Negative_prompt = usedSettings.NegativePrompt,
                Height = usedSettings.Height!.Value,
                Width = usedSettings.Width!.Value,
                Steps = usedSettings.Steps!.Value,
                Seed = usedSettings.Seed!.Value,
                Cfg_scale = usedSettings.CfgScale!.Value,
                Sampler_index = usedSettings.Sampler,
                Sampler_name = usedSettings.Sampler,
            }, cancellationToken).ConfigureAwait(false);

        return new TextToImageResponse
        {
            // base64 to png
            Images = response.Images
                .Select(Data.FromBase64)
                .ToArray(),
            Usage = Usage.Empty,
            UsedSettings = usedSettings,
        };
    }
}

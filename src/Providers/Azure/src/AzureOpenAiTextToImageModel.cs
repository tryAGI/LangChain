using Azure.AI.OpenAI;
using OpenAI.Constants;

namespace LangChain.Providers.Azure;

public class AzureOpenAiTextToImageModel : TextToImageModel, ITextToImageModel
{
    private readonly AzureOpenAiProvider _provider;
    private readonly ImageModels _model;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="id"></param>
    public AzureOpenAiTextToImageModel(AzureOpenAiProvider provider, string id)
        : base(id)
    {
        _provider = provider;
        _model = new(id);
    }

    /// <summary>
    /// Azure responds with a revised prompt if it changed it during generation, this property contains that prompt. Only relevant when Dall-E-3 model is used.
    /// </summary>
    public string RevisedPromptResult { get; set; } = string.Empty;

    /// <summary>
    /// Optional Image Generation Options, if null the default settings will be used
    /// NOTE: Currently only an ImageCount of 1 is supported, DALL-E-3 supports only 3 sizes 1024x1024, 1792X1024 or 1024x1792
    /// </summary>
    [CLSCompliant(false)]
    public ImageGenerationOptions? GenerationOptions { get; set; } = null;

    public async Task<TextToImageResponse> GenerateImageAsync(
        TextToImageRequest request,
        TextToImageSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        if (GenerationOptions != null && GenerationOptions.ImageCount != 1)
        {
            throw new NotSupportedException("Currently only 1 image is supported");
        }

        var usedSettings = OpenAiTextToImageSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: _provider.TextToImageSettings,
            defaultSettings: OpenAiTextToImageSettings.GetDefault(_model));

        var response = await _provider.Client.GetImageGenerationsAsync(GenerationOptions ?? new ImageGenerationOptions
        {
            DeploymentName = Id,
            ImageCount = 1, //currently hardcoded to 1
            Prompt = request.Prompt,
            Quality = new ImageGenerationQuality(usedSettings.Quality!.Value),
            Size = new ImageSize(usedSettings.Resolution!.Value), //DALL-E-3 supports only 3 sizes 1024x1024, 1792X1024 or 1024x1792
            Style = ImageGenerationStyle.Natural,
            User = usedSettings.User,
        }, cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            //Todo: Usage might be off when setting different parameters in GenerationOptions
            PriceInUsd = _model.GetPriceInUsd(
                resolution: usedSettings.Resolution!.Value,
                quality: usedSettings.Quality!.Value),
        };
        AddUsage(usage);

        var firstImage = response.Value.Data[0];
        RevisedPromptResult = firstImage.RevisedPrompt;

        var bytes = Convert.FromBase64String(
            firstImage.Base64Data ??
            throw new InvalidOperationException("B64_json is null"));

        return new TextToImageResponse
        {
            Bytes = bytes,
            Usage = usage,
            UsedSettings = usedSettings,
        };
    }
}

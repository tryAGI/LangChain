using Azure.AI.OpenAI;
using OpenAI.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Providers.Azure;

public partial class AzureOpenAIModel : IGenerateImageModel
{
    /// <summary>
    /// Azure responds with a revised prompt if it changed it during generation, this property contains that prompt. Only relevant when Dall-E-3 model is used.
    /// </summary>
    public string RevisedPromptResult { get; set; }

    /// <summary>
    /// Optional Image Generation Options, if null the default settings will be used
    /// NOTE: Currently only an ImageCount of 1 is supported, DALL-E-3 supports only 3 sizes 1024x1024, 1792X1024 or 1024x1792
    /// </summary>
    [CLSCompliant(false)]
    public ImageGenerationOptions? GenerationOptions { get; set; } = null;

    private async Task<ImageGenerationData> GenerateAndGetFirstImage(string prompt, CancellationToken cancellationToken)
    {
        if (GenerationOptions != null && GenerationOptions.ImageCount != 1)
        {
            throw new NotSupportedException("Currently only 1 image is supported");
        }
        var options = GenerationOptions ?? new ImageGenerationOptions
        {
            DeploymentName = Id,
            ImageCount = 1, //currently hardcoded to 1
            Prompt = prompt,
            Quality = ImageGenerationQuality.Standard,
            Size = ImageSize.Size1024x1024,  //DALL-E-3 supports only 3 sizes 1024x1024, 1792X1024 or 1024x1792
            Style = ImageGenerationStyle.Natural,
            User = User,
        };
        var response = await Client.GetImageGenerationsAsync(options, cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            //Todo: Usage might be off when setting different parameters in GenerationOptions
            PriceInUsd = ImageModels.DallE3.GetPriceInUsd(
                resolution: ImageResolutions._1024x1024,
                quality: ImageQualities.Standard),
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        var firstImage = response.Value.Data[0];
        this.RevisedPromptResult = firstImage.RevisedPrompt;
        return firstImage;
    }

    public async Task<byte[]> GenerateImageAsBytesAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ImageGenerationData firstImage = await GenerateAndGetFirstImage(prompt, cancellationToken).ConfigureAwait(false);

        return Convert.FromBase64String(
            firstImage.Base64Data ??
            throw new InvalidOperationException("B64_json is null"));

    }

    public async Task<Stream> GenerateImageAsStreamAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var bytes = await GenerateImageAsBytesAsync(
            prompt: prompt,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new MemoryStream(bytes);
    }

    public async Task<Uri> GenerateImageAsUrlAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ImageGenerationData firstImage = await GenerateAndGetFirstImage(prompt, cancellationToken).ConfigureAwait(false);

        return firstImage.Url;
    }
}

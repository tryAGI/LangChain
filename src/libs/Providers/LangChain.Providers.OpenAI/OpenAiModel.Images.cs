using OpenAI.Constants;
using OpenAI.Images;
using OpenAI.Models;

namespace LangChain.Providers.OpenAI;

public partial class OpenAiModel : IGenerateImageModel
{
    #region Methods

    /// <inheritdoc/>
    public async Task<Uri> GenerateImageAsUrlAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var response = await Api.ImagesEndPoint.GenerateImageAsync(
            request: new ImageGenerationRequest(
                prompt: prompt,
                model: Model.DallE_3,
                numberOfResults: 1,
                quality: ImageQualities.Standard,
                responseFormat: ResponseFormat.Url,
                size: ImageResolutions._256x256,
                user: User),
            cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = ImageModels.DallE3.GetPriceInUsd(
                resolution: ImageResolutions._256x256,
                quality: ImageQualities.Standard),
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return new Uri(
            response[0] ??
            throw new InvalidOperationException("Url is null"));
    }

    /// <inheritdoc/>
    public async Task<Stream> GenerateImageAsStreamAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var bytes = await GenerateImageAsBytesAsync(
            prompt: prompt,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new MemoryStream(bytes);
    }

    /// <inheritdoc/>
    public async Task<byte[]> GenerateImageAsBytesAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var response = await Api.ImagesEndPoint.GenerateImageAsync(
            request: new ImageGenerationRequest(
                prompt: prompt,
                model: Model.DallE_3,
                numberOfResults: 1,
                quality: ImageQualities.Standard,
                responseFormat: ResponseFormat.B64_Json,
                size: ImageResolutions._256x256,
                user: User),
            cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = ImageModels.DallE3.GetPriceInUsd(
                resolution: ImageResolutions._256x256,
                quality: ImageQualities.Standard),
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return Convert.FromBase64String(
            response[0] ??
            throw new InvalidOperationException("B64_json is null"));
    }

    #endregion
}
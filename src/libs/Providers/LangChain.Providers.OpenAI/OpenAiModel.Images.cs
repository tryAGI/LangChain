namespace LangChain.Providers.OpenAI;

public partial class OpenAiModel : IGenerateImageModel
{
    #region Methods

    /// <inheritdoc/>
    public async Task<Uri> GenerateImageAsUrlAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        const CreateImageRequestSize size = CreateImageRequestSize._256x256;
        var response = await Api.CreateImageAsync(new CreateImageRequest
        {
            Prompt = prompt,
            N = 1,
            Size = size,
            Response_format = CreateImageRequestResponse_format.Url,
            User = User,
        }, cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = ApiHelpers.CalculatePriceInUsd(size),
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return new Uri(
            response.Data.First().Url ??
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
        const CreateImageRequestSize size = CreateImageRequestSize._256x256;
        var response = await Api.CreateImageAsync(new CreateImageRequest
        {
            Prompt = prompt,
            N = 1,
            Size = size,
            Response_format = CreateImageRequestResponse_format.B64_json,
            User = User,
        }, cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = ApiHelpers.CalculatePriceInUsd(size),
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return Convert.FromBase64String(
            response.Data.First().B64_json ??
            throw new InvalidOperationException("B64_json is null"));
    }

    #endregion
}
namespace LangChain.Providers;

public partial class LeonardoAiModel : IGenerateImageModel
{
    #region Methods

    /// <inheritdoc/>
    public async Task<Uri> GenerateImageAsUrlAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var createResponse = await Api.CreateGenerationAsync(new Body
        {
            Prompt = "Generate cat",
        }, cancellationToken).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken).ConfigureAwait(false);

        var response = await Api.GetGenerationByIdAsync(
            id: createResponse.SdGenerationJob?.GenerationId ??
                throw new InvalidOperationException("Id is null."), cancellationToken).ConfigureAwait(false);
        var url =
            response.Generations_by_pk?.Generated_images?.ElementAtOrDefault(0)?.Url ??
            throw new InvalidOperationException("Url is null.");

        return new Uri(url);
    }

    /// <inheritdoc/>
    public async Task<Stream> GenerateImageAsStreamAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var uri = await GenerateImageAsUrlAsync(
            prompt: prompt,
            cancellationToken: cancellationToken).ConfigureAwait(false);

#if NET6_0_OR_GREATER
        return await HttpClient.GetStreamAsync(uri, cancellationToken).ConfigureAwait(false);
#else
        return await HttpClient.GetStreamAsync(uri).ConfigureAwait(false);
#endif
    }

    /// <inheritdoc/>
    public async Task<byte[]> GenerateImageAsBytesAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var uri = await GenerateImageAsUrlAsync(
            prompt: prompt,
            cancellationToken: cancellationToken).ConfigureAwait(false);

#if NET6_0_OR_GREATER
        return await HttpClient.GetByteArrayAsync(uri, cancellationToken).ConfigureAwait(false);
#else
        return await HttpClient.GetByteArrayAsync(uri).ConfigureAwait(false);
#endif
    }

    #endregion
}
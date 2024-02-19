namespace LangChain.Providers.LeonardoAi;

public class LeonardoAiModel(
    LeonardoAiProvider provider,
    string id)
    : ImageGenerationModel(id), IImageGenerationModel
{
    #region Methods

    /// <inheritdoc/>
    public async Task<ImageGenerationResponse> GenerateImageAsync(
        ImageGenerationRequest request,
        ImageGenerationSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        OnPromptSent(request.Prompt);
        
        var createResponse = await provider.Api.CreateGenerationAsync(new Body
        {
            Prompt = request.Prompt,
        }, cancellationToken).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken).ConfigureAwait(false);

        var response = await provider.Api.GetGenerationByIdAsync(
            id: createResponse.SdGenerationJob?.GenerationId ??
                throw new InvalidOperationException("Id is null."), cancellationToken).ConfigureAwait(false);
        var url =
            response.Generations_by_pk?.Generated_images?.ElementAtOrDefault(0)?.Url ??
            throw new InvalidOperationException("Url is null.");

#if NET6_0_OR_GREATER
        var bytes = await provider.HttpClient.GetByteArrayAsync(new Uri(url), cancellationToken).ConfigureAwait(false);
#else
        var bytes = await provider.HttpClient.GetByteArrayAsync(new Uri(url)).ConfigureAwait(false);
#endif
        
        return new ImageGenerationResponse
        {
            Bytes = bytes,
            Usage = Usage.Empty,
            UsedSettings = settings ?? ImageGenerationSettings.Default,
        };
    }
    
    #endregion
}
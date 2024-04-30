namespace LangChain.Providers.LeonardoAi;

public class LeonardoAiModel(
    LeonardoAiProvider provider,
    string id)
    : TextToImageModel(id), ITextToImageModel
{
    #region Methods

    /// <inheritdoc/>
    public async Task<TextToImageResponse> GenerateImageAsync(
        TextToImageRequest request,
        TextToImageSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        OnPromptSent(request.Prompt);

        var createResponse = await provider.Api.CreateGenerationAsync(new Body
        {
            Prompt = request.Prompt,
        }, cancellationToken).ConfigureAwait(false);

        ICollection<Generated_images> images = new List<Generated_images>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await provider.Api.GetGenerationByIdAsync(
                id: createResponse.SdGenerationJob?.GenerationId ??
                    throw new InvalidOperationException("Id is null."), cancellationToken).ConfigureAwait(false);
            if (response.Generations_by_pk?.Status == Job_status.FAILED)
            {
                throw new InvalidOperationException($"Generation failed.");
            }
            if (response.Generations_by_pk?.Status == Job_status.COMPLETE)
            {
                images =
                    response.Generations_by_pk?.Generated_images ??
                    throw new InvalidOperationException("Generated_images is null.");
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
        }

        return new TextToImageResponse
        {
            Images = await Task.WhenAll(images
                .Where(x => x.Url != null)
                .Select(async x =>
                {
                    var bytes = await provider.HttpClient.GetByteArrayAsync(new Uri(x.Url!), cancellationToken).ConfigureAwait(false);

                    return Data.FromBytes(bytes);
                })).ConfigureAwait(false),
            Usage = Usage.Empty,
            UsedSettings = settings ?? TextToImageSettings.Default,
        };
    }

    #endregion
}
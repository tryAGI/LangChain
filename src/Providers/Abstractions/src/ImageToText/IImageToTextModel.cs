namespace LangChain.Providers;

/// <summary>
/// Defines a large language model that can be used for image to text generation.
/// </summary>
public interface IImageToTextModel : IModel<ImageToTextSettings>
{
    /// <summary>
    /// Run the LLM on the given image.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ImageToTextResponse> GenerateTextFromImageAsync(
        ImageToTextRequest request,
        ImageToTextSettings? settings = null,
        CancellationToken cancellationToken = default);
}
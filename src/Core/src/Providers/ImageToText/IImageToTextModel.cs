namespace LangChain.Providers;

/// <summary>
/// Defines a model that can generate text from images.
/// </summary>
public interface IImageToTextModel : IModel<ImageToTextSettings>
{
    /// <summary>
    /// Generates text from the given image.
    /// </summary>
    public Task<ImageToTextResponse> GenerateTextFromImageAsync(
        ImageToTextRequest request,
        ImageToTextSettings? settings = null,
        CancellationToken cancellationToken = default);
}

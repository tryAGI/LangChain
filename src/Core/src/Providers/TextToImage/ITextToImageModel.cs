namespace LangChain.Providers;

/// <summary>
/// Defines a model that can generate images from text.
/// </summary>
public interface ITextToImageModel : IModel<TextToImageSettings>
{
    /// <summary>
    /// Occurs before prompt is sent to the model.
    /// </summary>
    event EventHandler<TextToImageRequest>? RequestSent;

    /// <summary>
    /// Generates an image from a text prompt.
    /// </summary>
    public Task<TextToImageResponse> GenerateImageAsync(
        TextToImageRequest request,
        TextToImageSettings? settings = default,
        CancellationToken cancellationToken = default);
}

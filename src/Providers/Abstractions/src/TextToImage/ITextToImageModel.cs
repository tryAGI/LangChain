// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Defines the interface for models that generate images based on textual input. Implementations of this interface should provide mechanisms to interpret text and produce corresponding visual content.
/// </summary>
public interface ITextToImageModel : IModel<TextToImageSettings>
{
    /// <summary>
    /// Occurs before a prompt is sent to the model, indicating the initiation of the image generation process based on the provided text.
    /// </summary>
    event EventHandler<string>? PromptSent;

    /// <summary>
    /// Asynchronously generates an image based on the provided text request and settings.
    /// </summary>
    /// <param name="request">The text to image request containing the text and any additional information required for image generation.</param>
    /// <param name="settings">Optional settings to customize the image generation process. If null, default settings are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in a TextToImageResponse object containing the generated image.</returns>
    public Task<TextToImageResponse> GenerateImageAsync(
        TextToImageRequest request,
        TextToImageSettings? settings = default,
        CancellationToken cancellationToken = default);
}
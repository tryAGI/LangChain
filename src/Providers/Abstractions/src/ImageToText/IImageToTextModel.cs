// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// Defines a large language model interface for converting images to text. This interface outlines the contract for models that can interpret visual content and generate corresponding textual descriptions.
/// </summary>
public interface IImageToTextModel : IModel<ImageToTextSettings>
{
    /// <summary>
    /// Run the LLM on the given image.
    /// </summary>
    /// <param name="request">The image to text request containing the image and any additional information required for text generation.</param>
    /// <param name="settings">Optional settings to customize the text generation process. If null, default settings are used.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in an ImageToTextResponse object containing the generated text.</returns>
    public Task<ImageToTextResponse> GenerateTextFromImageAsync(
        ImageToTextRequest request,
        ImageToTextSettings? settings = null,
        CancellationToken cancellationToken = default);
}
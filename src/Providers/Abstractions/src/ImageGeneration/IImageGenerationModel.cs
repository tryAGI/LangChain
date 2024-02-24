// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public interface IImageGenerationModel : IModel<ImageGenerationSettings>
{
    /// <summary>
    /// Occurs before prompt is sent to the model.
    /// </summary>
    event EventHandler<string>? PromptSent;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ImageGenerationResponse> GenerateImageAsync(
        ImageGenerationRequest request,
        ImageGenerationSettings? settings = default,
        CancellationToken cancellationToken = default);
}
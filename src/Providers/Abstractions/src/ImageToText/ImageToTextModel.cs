// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

public abstract class ImageToTextModel(string id) : Model<ImageToTextSettings>(id), IImageToTextModel<ImageToTextRequest, ImageToTextResponse, ImageToTextSettings>
{
    public abstract Task<ImageToTextResponse> GenerateTextFromImageAsync(
        ImageToTextRequest request,
        ImageToTextSettings? settings = default,
        CancellationToken cancellationToken = default);
}
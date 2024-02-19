using OpenAI.Constants;
using OpenAI.Images;
using NotImplementedException = System.NotImplementedException;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

/// <summary>
/// 
/// </summary>
/// <param name="provider"></param>
/// <param name="id"></param>
public class OpenAiImageGenerationModel(
    OpenAiProvider provider,
    string id)
    : ImageGenerationModel(id), IImageGenerationModel
{
    /// <inheritdoc/>
    public async Task<ImageGenerationResponse> GenerateImageAsync(
        ImageGenerationRequest request,
        ImageGenerationSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        OnPromptSent(request.Prompt);

        var usedSettings = OpenAiImageGenerationSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ImageGenerationSettings);
        var response = await provider.Api.ImagesEndPoint.GenerateImageAsync(
            request: new global::OpenAI.Images.ImageGenerationRequest(
                prompt: request.Prompt,
                model: new global::OpenAI.Models.Model(Id, "openai"),
                numberOfResults: usedSettings.NumberOfResults!.Value,
                quality: usedSettings.Quality!,
                responseFormat: usedSettings.ResponseFormat!.Value,
                size: usedSettings.Resolution!,
                user: usedSettings.User!),
            cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = ImageModels.DallE3.GetPriceInUsd(
                resolution: usedSettings.Resolution!.Value,
                quality: usedSettings.Quality),
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        switch (usedSettings.ResponseFormat)
        {
            case ResponseFormat.Url:
            {
                using var client = new HttpClient();
#if NET6_0_OR_GREATER
                var bytes = await client.GetByteArrayAsync(new Uri(response[0].Url), cancellationToken).ConfigureAwait(false);
#else
                var bytes = await client.GetByteArrayAsync(new Uri(response[0].Url)).ConfigureAwait(false);
#endif
                
                return new ImageGenerationResponse
                {
                    Bytes = bytes,
                    Usage = usage,
                    UsedSettings = usedSettings,
                };
            }
            
            case ResponseFormat.B64_Json:
                return new ImageGenerationResponse
                {
                    Bytes = Convert.FromBase64String(
                        response[0].B64_Json ??
                        throw new InvalidOperationException("B64_json is null")),
                    Usage = usage,
                    UsedSettings = usedSettings,
                };
            
            default:
                throw new NotImplementedException("ResponseFormat not implemented.");
        }
    }
}
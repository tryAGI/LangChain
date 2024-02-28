using OpenAI.Constants;
using OpenAI.Images;
using NotImplementedException = System.NotImplementedException;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;


public class OpenAiTextToImageModel : TextToImageModel, ITextToImageModel
{
    private readonly OpenAiProvider _provider;
    private readonly ImageModels _model;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="id"></param>
    public OpenAiTextToImageModel(OpenAiProvider provider, string id)
        : base(id)
    {
        _provider = provider;
        _model = new(id);
    }

    /// <inheritdoc/>
    public async Task<TextToImageResponse> GenerateImageAsync(
        TextToImageRequest request,
        TextToImageSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        OnPromptSent(request.Prompt);

        var usedSettings = OpenAiTextToImageSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: _provider.TextToImageSettings,
            defaultSettings: OpenAiTextToImageSettings.GetDefaultSettings(_model));

        var response = await _provider.Api.ImagesEndPoint.GenerateImageAsync(
            request: new ImageGenerationRequest(
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
            PriceInUsd = _model.GetPriceInUsd(
                resolution: usedSettings.Resolution!.Value,
                quality: usedSettings.Quality),
        };
        AddUsage(usage);
        _provider.AddUsage(usage);

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

                    return new TextToImageResponse
                    {
                        Bytes = bytes,
                        Usage = usage,
                        UsedSettings = usedSettings,
                    };
                }

            case ResponseFormat.B64_Json:
                return new TextToImageResponse
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
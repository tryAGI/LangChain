// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

/// <summary>
/// 
/// </summary>
/// <param name="provider"></param>
/// <param name="id"></param>
public class OpenAiTextToImageModel(
    OpenAiProvider provider,
    string id)
    : TextToImageModel(id), ITextToImageModel
{
    [CLSCompliant(false)]
    public OpenAiTextToImageModel(
        OpenAiProvider provider,
        CreateImageRequestModel id)
        : this(provider, id.ToValueString())
    {
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
            providerSettings: provider.TextToImageSettings,
            defaultSettings: OpenAiTextToImageSettings.GetDefaultSettings(Id));

        var response = await provider.Api.Images.CreateImageAsync(
            prompt: request.Prompt,
            model: Id,
            n: usedSettings.NumberOfResults!.Value,
            quality: usedSettings.Quality!,
            responseFormat: usedSettings.ResponseFormat!.Value,
            size: usedSettings.Size!,
            style: usedSettings.Style!,
            user: usedSettings.User!,
            cancellationToken).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            PriceInUsd = CreateImageRequestModelExtensions.ToEnum(Id)?.GetPriceInUsd(
                size: usedSettings.Size!.Value,
                quality: usedSettings.Quality) ?? double.NaN,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        switch (usedSettings.ResponseFormat)
        {
            case CreateImageRequestResponseFormat.Url:
                {
                    using var client = new HttpClient();
                    var images = await Task.WhenAll(response.Data.Select(async x =>
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        var bytes = await client.GetByteArrayAsync(new Uri(x.Url!), cancellationToken).ConfigureAwait(false);

                        return Data.FromBytes(bytes);
                    })).ConfigureAwait(false);

                    return new TextToImageResponse
                    {
                        Images = images,
                        Usage = usage,
                        UsedSettings = usedSettings,
                    };
                }

            case CreateImageRequestResponseFormat.B64Json:
                return new TextToImageResponse
                {
                    Images = response.Data
                        .Select(static x =>
                            Data.FromBase64(x.B64Json ??
                                            throw new InvalidOperationException("B64_json is null")))
                        .ToArray(),
                    Usage = usage,
                    UsedSettings = usedSettings,
                };

            default:
                throw new NotImplementedException("ResponseFormat not implemented.");
        }
    }
}
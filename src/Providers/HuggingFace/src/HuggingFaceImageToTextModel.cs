using System.Diagnostics;
using System.Text.Json;

namespace LangChain.Providers.HuggingFace;

/// <summary>
/// 
/// </summary>
public class HuggingFaceImageToTextModel(
    HuggingFaceProvider provider,
    string id)
    : ImageToTextModel(id), IImageToTextModel
{
    public override async Task<ImageToTextResponse> GenerateTextFromImageAsync(ImageToTextRequest request, ImageToTextSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var watch = Stopwatch.StartNew();

        var usedSettings = ImageToTextSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ImageToTextSettings);

        var imageContent = new ByteArrayContent(request.Image.ToArray());
        if (request.Image.MediaType != null) imageContent.Headers.ContentType = new(request.Image.MediaType);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, usedSettings.Endpoint + id)
        {
            Content = imageContent
        };

        var response = await provider.HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var deserializeResponse = DeserializeResponse<ImageToTextGenerationResponse>(body);

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ImageToTextResponse
        {
            Text = deserializeResponse.SingleOrDefault()?.GeneratedText,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }

    private static T DeserializeResponse<T>(string body)
    {
        body = body ?? throw new ArgumentNullException(nameof(body));

        T? deserializedResponse = JsonSerializer.Deserialize<T>(body);
        if (deserializedResponse is null)
        {
            throw new JsonException("Response is null");
        }

        return deserializedResponse;
    }
}
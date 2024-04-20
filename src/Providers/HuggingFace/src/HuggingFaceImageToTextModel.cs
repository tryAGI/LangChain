using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangChain.Providers.HuggingFace;

/// <summary>
/// 
/// </summary>
public class HuggingFaceImageToTextModel(
    HuggingFaceProvider provider,
    string id)
    : ImageToTextModel(id), IImageToTextModel
{
    public override async Task<ImageToTextResponse> GenerateTextFromImageAsync(
        ImageToTextRequest request,
        ImageToTextSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var watch = Stopwatch.StartNew();

        var usedSettings = HuggingFaceImageToTextSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ImageToTextSettings);

        var imageContent = new ByteArrayContent(request.Image.ToArray());
        if (request.Image.MediaType != null)
        {
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(request.Image.MediaType);
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, usedSettings.Endpoint + Id);
        httpRequest.Content = imageContent;

        var response = await provider.HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var generation = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.ImageToTextGenerationResponse) ??
                         throw new InvalidOperationException("Response is null");

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ImageToTextResponse
        {
            Text = generation.SingleOrDefault()?.GeneratedText,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }
}

[JsonSerializable(typeof(ImageToTextGenerationResponse))]
public partial class SourceGenerationContext : JsonSerializerContext;
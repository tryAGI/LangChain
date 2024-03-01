using System.Diagnostics;
using System.Net;
using System.Text.Json;
using static LangChain.Providers.HuggingFace.ImageToTextGenerationResponse;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace LangChain.Providers.HuggingFace;

/// <summary>
/// 
/// </summary>
public class HuggingFaceImageToTextModel(
    HuggingFaceProvider provider,
    string id)
    : ImageToTextModel(id), IImageToTextModel
{
    public Usage Usage { get; }
    public void AddUsage(Usage usage)
    {
        throw new NotImplementedException();
    }

    public string Endpoint { get; set; } = "https://api-inference.huggingface.co/models/";

    public ImageToTextSettings? Settings { get; init; }

    public override async Task<ImageToTextResponse> GenerateTextFromImageAsync(ImageToTextRequest request, ImageToTextSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        var imageContent = new ByteArrayContent(request.Image.ToArray());
        imageContent.Headers.ContentType = new(request.Image.MediaType);

        var request2 = new HttpRequestMessage(HttpMethod.Post, Endpoint + id)
        {
            Content = imageContent
        };

        var response = await provider.HttpClient.SendAsync(request2, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        ImageToTextGenerationResponse response2 = DeserializeResponse<ImageToTextGenerationResponse>(body);
        Console.WriteLine(response2[0].GeneratedText);
        return null;
    }

    private static T DeserializeResponse<T>(string body)
    {
        try
        {
            T? deserializedResponse = JsonSerializer.Deserialize<T>(body);
            if (deserializedResponse is null)
            {
                throw new JsonException("Response is null");
            }

            return deserializedResponse;
        }
        catch (JsonException exc)
        {
            throw;
        }
    }
}

internal sealed class ImageToTextGenerationResponse : List<GeneratedTextItem>
{
    internal sealed class GeneratedTextItem
    {
        /// <summary>
        /// The continuated string
        /// </summary>
        [JsonPropertyName("generated_text")]
        public string? GeneratedText { get; set; }
    }
}
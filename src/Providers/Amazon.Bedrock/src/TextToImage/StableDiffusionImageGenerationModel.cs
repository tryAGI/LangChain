using System.Diagnostics;
using System.Text;
using System.Text.Json;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class StableDiffusionTextToImageModel(
    BedrockProvider provider,
    string id)
    : TextToImageModel(id), ITextToImageModel
{
    public async Task<TextToImageResponse> GenerateImageAsync(
        TextToImageRequest request,
        TextToImageSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var watch = Stopwatch.StartNew();
        string[] prompts = [request.Prompt];

        var response = await provider.Api.InvokeModelAsync(
            Id,
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
            {
                text_prompts = prompts
                    .Select(static prompt => new { text = prompt })
                    .ToArray(),
            })),
            cancellationToken).ConfigureAwait(false);

        var base64 = response?["artifacts"]?[0]?["base64"]?
            .GetValue<string>() ?? string.Empty;
        
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new TextToImageResponse
        {
            Images = [Data.FromBase64(base64)],
            UsedSettings = TextToImageSettings.Default,
            Usage = usage,
        };
    }
}
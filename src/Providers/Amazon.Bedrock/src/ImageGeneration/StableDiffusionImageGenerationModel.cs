using System.Diagnostics;
using System.Text;
using System.Text.Json;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class StableDiffusionImageGenerationModel(
    BedrockProvider provider,
    string id)
    : ImageGenerationModel(id), IImageGenerationModel
{
    public async Task<ImageGenerationResponse> GenerateImageAsync(
        ImageGenerationRequest request,
        ImageGenerationSettings? settings = null,
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
        //var generatedText = $"data:image/jpeg;base64,{body}";
        
        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ImageGenerationResponse
        {
            Bytes = Convert.FromBase64String(base64),
            UsedSettings = ImageGenerationSettings.Default,
            Usage = usage,
        };
    }
}
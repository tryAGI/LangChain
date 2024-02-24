using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AmazonTitanImageGenerationModel(
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
        var response = await provider.Api.InvokeModelAsync(
            Id,
            new JsonObject
            {
                ["taskType"] = "TEXT_IMAGE",
                ["textToImageParams"] = new JsonObject
                {
                    ["text"] = request.Prompt,
                    ["imageGenerationConfig"] = new JsonObject
                    {
                        ["quality"] = "standard",
                        ["width"] = 1024,
                        ["height"] = 1024,
                        ["cfgScale"] = 8.0,
                        ["seed"] = 0,
                        ["numberOfImages"] = 3,
                    }
                }
            },
            cancellationToken).ConfigureAwait(false);

        var generatedText = response?["results"]?[0]?["outputText"]?.GetValue<string>() ?? "";
        
        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ImageGenerationResponse
        {
            Bytes = Convert.FromBase64String(generatedText),
            UsedSettings = ImageGenerationSettings.Default,
            Usage = usage,
        };
    }
}
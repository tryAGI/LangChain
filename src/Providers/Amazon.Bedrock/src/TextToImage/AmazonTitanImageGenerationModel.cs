using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AmazonTitanTextToImageModel(
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

        var usedSettings = BedrockImageSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.TextToImageSettings);
        var response = await provider.Api.InvokeModelAsync(
            Id,
            new JsonObject
            {
                ["taskType"] = "TEXT_IMAGE",
                ["textToImageParams"] = new JsonObject
                {
                    ["text"] = request.Prompt
                },
                ["imageGenerationConfig"] = new JsonObject
                {
                    ["quality"] = "standard",
                    ["width"] = usedSettings.Width!.Value,
                    ["height"] = usedSettings.Height!.Value,
                    ["cfgScale"] = 8.0,
                    ["seed"] = usedSettings.Seed!.Value,
                    ["numberOfImages"] = usedSettings.NumOfImages!.Value,
                }
            },
            cancellationToken).ConfigureAwait(false);

        var generatedText = response?["images"]?[0]?.GetValue<string>() ?? "";

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new TextToImageResponse
        {
            Bytes = Convert.FromBase64String(generatedText),
            UsedSettings = TextToImageSettings.Default,
            Usage = usage,
        };
    }
}
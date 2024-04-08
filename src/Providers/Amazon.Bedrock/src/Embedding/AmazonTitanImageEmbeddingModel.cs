using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public abstract class AmazonTitanImageEmbeddingModel(
    BedrockProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{
    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var watch = Stopwatch.StartNew();

        var usedSettings = BedrockEmbeddingSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.EmbeddingSettings);

        var splitText = request.Strings.Split(chunkSize: (int)usedSettings.MaximumInputLength!);
        var embeddings = new List<float[]>(capacity: splitText.Count);

        var bodyJson = CreateBodyJson(string.Join(" ", splitText), request.Images);

        var response = await provider.Api.InvokeModelAsync(Id, bodyJson, cancellationToken).ConfigureAwait(false);
        var embedding = response?["embedding"]?.AsArray();

        var f = new float[(int)usedSettings.Dimensions!];
        for (var i = 0; i < embedding!.Count; i++)
        {
            f[i] = (float)embedding[(Index)i]?.AsValue()!;
        }

        embeddings.Add(f);

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new EmbeddingResponse
        {
            Values = embeddings.ToArray(),
            Usage = Usage.Empty,
            UsedSettings = usedSettings,
        };
    }

    private static JsonObject CreateBodyJson(
        string? prompt,
        IList<Data>? images = null)
    {
        var bodyJson = new JsonObject
        {
            ["inputText"] = string.IsNullOrEmpty(prompt) ? "describe image" : prompt,
        };

        if (images!.Count <= 0) return bodyJson;

        var base64 = Convert.ToBase64String(images[0]);
        bodyJson.Add("inputImage", base64);

        return bodyJson;
    }
}
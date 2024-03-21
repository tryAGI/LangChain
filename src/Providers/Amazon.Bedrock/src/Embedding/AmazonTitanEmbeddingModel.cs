using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public abstract class AmazonTitanEmbeddingModel(
    BedrockProvider provider,
    string id)
    : Model<EmbeddingSettings>(id), IEmbeddingModel
{
    public int MaximumInputLength => 10_000;

    public async Task<EmbeddingResponse> CreateEmbeddingsAsync(
        EmbeddingRequest request,
        EmbeddingSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var watch = Stopwatch.StartNew();
        var splitText = request.Strings.Split(chunkSize: MaximumInputLength);
        var tasks = new List<Task<JsonNode>>();

        var embeddings = new List<float[]>(capacity: splitText.Count);
        foreach (var text in splitText)
        {
            Task<JsonNode> task = provider.Api.InvokeModelAsync(Id, new JsonObject
            {
                { "inputText", text },
            }, cancellationToken);

            tasks.Add(task);
        }

        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        foreach (var response in results)
        {
            var embedding = response?["embedding"].AsArray();

            var f = new float[1536];
            if (embedding != null)
            {
                for (var i = 0; i < embedding.Count; i++)
                {
                    f[i] = (float)embedding[(Index)i]?.AsValue()!;
                }
            }

            embeddings.Add(f);
        }

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new EmbeddingResponse
        {
            // TODO: Check this place
            Values = embeddings.ToArray(),
            Usage = Usage.Empty,
            UsedSettings = EmbeddingSettings.Default,
        };
    }
}
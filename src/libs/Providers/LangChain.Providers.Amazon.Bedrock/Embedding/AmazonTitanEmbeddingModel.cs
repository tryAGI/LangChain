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

        // TODO: Can it be done in parallel?
        var embeddings = new List<float>(capacity: splitText.Count);
        foreach (var text in splitText)
        {
            var response = await provider.Api.InvokeModelAsync(Id, new JsonObject
            {
                { "inputText", text },
            }, cancellationToken).ConfigureAwait(false);

            embeddings.AddRange(response?["embedding"]?
                .AsArray()
                .Select(x => (float?)x?.AsValue() ?? 0.0f)
                .ToArray() ?? []);
        }

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new EmbeddingResponse
        {
            // TODO: Check this place
            Values = embeddings
                .Select(f => new[] { f })
                .ToArray(),
            Usage = Usage.Empty,
            UsedSettings = EmbeddingSettings.Default,
        };
    }
}
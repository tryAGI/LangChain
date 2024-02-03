using System.Diagnostics;

namespace LangChain.Providers.Bedrock.Embeddings;

public class BedrockEmbeddings : BedrockEmbeddingsBase
{
    private readonly BedrockEmbeddingsConfiguration _configuration;

    public BedrockEmbeddings(string modelId)
    {
        Id = modelId ?? throw new ArgumentException("ModelId is not defined", nameof(modelId));
        _configuration = new BedrockEmbeddingsConfiguration { ModelId = modelId };
    }

    public override async Task<float[][]> EmbedDocumentsAsync(string[] texts, CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();

        var response = await CreateCompletionAsync(texts, _configuration, cancellationToken).ConfigureAwait(false);

        watch.Stop();

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return response;
    }

    public override async Task<float[]> EmbedQueryAsync(string text, CancellationToken cancellationToken = default)
    {
        var watch = Stopwatch.StartNew();

        var response = await CreateCompletionAsync(text, _configuration, cancellationToken).ConfigureAwait(false);

        watch.Stop();

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return response;
    }
}
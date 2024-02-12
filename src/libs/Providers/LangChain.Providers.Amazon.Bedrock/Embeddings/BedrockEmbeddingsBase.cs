using Amazon.BedrockRuntime;
using LangChain.Abstractions.Embeddings.Base;

namespace LangChain.Providers.Amazon.Bedrock.Embeddings;

public abstract class BedrockEmbeddingsBase : IEmbeddings
{
    protected readonly Dictionary<string, Func<IBedrockEmbeddingsRequest>> _requestTypes = new()
    {
        { AmazonModelIds.AmazonTitanEmbeddingsG1TextV1, () => new AmazonTitanEmbeddingsRequest() },
        { AmazonModelIds.AmazonTitanMultiModalEmbeddingsG1V1, () => new AmazonTitanMultiModalEmbeddingsRequest() },

        { AmazonModelIds.CohereEmbedEnglish, () => new CohereEmbeddingsRequest() },
        { AmazonModelIds.CohereEmbedMultilingual, () => new CohereEmbeddingsRequest() },
    };
    private protected readonly object _usageLock = new();

    public abstract Task<float[][]> EmbedDocumentsAsync(string[] texts, CancellationToken cancellationToken = default);
    public abstract Task<float[]> EmbedQueryAsync(string text, CancellationToken cancellationToken = default);

    public string Id { get; set; }
    public Usage TotalUsage { get; set; }

    protected async Task<float[][]> CreateCompletionAsync(
        string[] texts,
        BedrockEmbeddingsConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var bedrockEmbeddingsRequest = _requestTypes[Id]();
        var client = new AmazonBedrockRuntimeClient(configuration.Region);
        var response = await bedrockEmbeddingsRequest.EmbedDocumentsAsync(client, texts, configuration);

        return response;
    }

    protected async Task<float[]> CreateCompletionAsync(
        string text,
        BedrockEmbeddingsConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var bedrockEmbeddingsRequest = _requestTypes[Id]();
        var client = new AmazonBedrockRuntimeClient(configuration.Region);
        var response = await bedrockEmbeddingsRequest.EmbedQueryAsync(client, text, configuration);

        return response;
    }
}
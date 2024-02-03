using Amazon.BedrockRuntime;
using LangChain.Abstractions.Embeddings.Base;
using LangChain.Providers.Bedrock.Models;

namespace LangChain.Providers.Bedrock.Embeddings;

public abstract class BedrockEmbeddingsBase : IEmbeddings
{
    protected readonly Dictionary<string, Func<IBedrockEmbeddingsRequest>> _requestTypes;
    private protected readonly object _usageLock = new();

    public BedrockEmbeddingsBase()
    {
        _requestTypes = new Dictionary<string, Func<IBedrockEmbeddingsRequest>>
        {
            { "amazon.titan-embed-text-v1", () => new AmazonTitanEmbeddingsRequest() },
            // { "amazon.titan-embed-image-v1", () => new AmazonTitanEmbeddingsRequest() },     // TODO

            // { "cohere.embed-english-v3", () => new CohereEmbeddingsRequest() },              // TODO
            // { "cohere.embed-multilingual-v3", () => new CohereEmbeddingsRequest() },         // TODO
        };
    }

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
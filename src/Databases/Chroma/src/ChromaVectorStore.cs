using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LangChain.Common.Converters;
using LangChain.Docstore;
using LangChain.Providers;
using LangChain.VectorStores;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.Memory.Chroma;
using Microsoft.SemanticKernel.Connectors.Memory.Chroma.Http.ApiSchema;
using Microsoft.SemanticKernel.Memory;

namespace LangChain.Databases;

/// <summary>
/// ChromaDB vector store.
/// According: https://api.python.langchain.com/en/latest/_modules/langchain/vectorstores/chroma.html
/// </summary>
[RequiresDynamicCode("Requires dynamic code.")]
[RequiresUnreferencedCode("Requires unreferenced code.")]
public class ChromaVectorStore : VectorStore
{
    private const string LangchainDefaultCollectionName = "langchain";

    // TODO: SemanticKernel impl doesn't support collection metadata. Need changes when moved to another impl
    private Dictionary<string, string>? CollectionMetadata { get; } = new();

    private readonly ChromaMemoryStore _store;

    private readonly ChromaClient _client;
    private readonly string _collectionName;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <inheritdoc />
    public ChromaVectorStore(
        HttpClient httpClient,
        string endpoint,
        IEmbeddingModel embeddings,
        string collectionName = LangchainDefaultCollectionName)
        : base(embeddings)
    {
        _client = new ChromaClient(httpClient, endpoint);

        _collectionName = collectionName;

        _store = new ChromaMemoryStore(_client);

        _client.CreateCollectionAsync(_collectionName).GetAwaiter().GetResult();

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new ObjectAsPrimitiveConverter(
                    floatFormat: FloatFormat.Double,
                    unknownNumberFormat: UnknownNumberFormat.Error,
                    objectFormat: ObjectFormat.Expando)
            },
            WriteIndented = true,
        };
    }

    /// <summary>
    /// Get collection
    /// </summary>
    [CLSCompliant(false)]
    public async Task<ChromaCollectionModel?> GetCollectionAsync()
    {
        return await _client.GetCollectionAsync(_collectionName).ConfigureAwait(false);
    }

    /// <summary>
    /// Delete collection
    /// </summary>
    public async Task DeleteCollectionAsync()
    {
        await _store.DeleteCollectionAsync(_collectionName).ConfigureAwait(false);
    }

    /// <summary>
    /// Get collection
    /// </summary>
    public async Task<Document?> GetDocumentByIdAsync(string id)
    {
        var record = await _store.GetAsync(_collectionName, id, withEmbedding: true).ConfigureAwait(false);

        if (record == null)
        {
            return null;
        }

        var text = record.Metadata.Text;
        var metadata = DeserializeMetadata(record.Metadata);

        return new Document(text, metadata);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddDocumentsAsync(
        IEnumerable<Document> documents,
        CancellationToken cancellationToken = default)
    {
        var documentsArray = documents.ToArray();
        var texts = new string[documentsArray.Length];
        var ids = new string[documentsArray.Length];
        var metadatas = new Dictionary<string, object>[documentsArray.Length];
        for (var index = 0; index < documentsArray.Length; index++)
        {
            ids[index] = Guid.NewGuid().ToString();
            texts[index] = documentsArray[index].PageContent;
            metadatas[index] = documentsArray[index].Metadata;
        }

        var result = await AddCoreAsync(texts, metadatas, ids, cancellationToken).ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddTextsAsync(
        IEnumerable<string> texts,
        IEnumerable<Dictionary<string, object>>? metadatas = null,
        CancellationToken cancellationToken = default)
    {
        var textsArray = texts.ToArray();
        var metadatasArray = metadatas?.ToArray() ?? new Dictionary<string, object>?[textsArray.Length];
        var ids = new string[textsArray.Length];

        for (var index = 0; index < textsArray.Length; index++)
        {
            ids[index] = Guid.NewGuid().ToString();
            metadatasArray[index] ??= new Dictionary<string, object>();
        }

        var result = await AddCoreAsync(
                textsArray,
                metadatasArray as Dictionary<string, object>[],
                ids,
                cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        await _store.RemoveBatchAsync(_collectionName, ids, cancellationToken).ConfigureAwait(false);

        return true;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(
        IEnumerable<float> embedding,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var documentsWithScores = await SimilaritySearchByVectorWithAsync(embedding.ToArray(), k, cancellationToken).ConfigureAwait(false);
        var documents = documentsWithScores.Select(dws => dws.Item1);

        return documents;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await EmbeddingModel
            .CreateEmbeddingsAsync(query, null, cancellationToken)
            .ConfigureAwait(false);

        var documentsWithScores = await SimilaritySearchByVectorWithAsync(embeddings, k, cancellationToken).ConfigureAwait(false);

        return documentsWithScores;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(
        IEnumerable<float> embedding,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
        => Task.FromException<IEnumerable<Document>>(new NotSupportedException("Querying not supported by SemanticKernel impl."));

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(
        string query,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        float[] embeddings = await EmbeddingModel
            .CreateEmbeddingsAsync(query, null, cancellationToken)
            .ConfigureAwait(false);

        var documents = await MaxMarginalRelevanceSearchByVector(
                embeddings,
                k,
                fetchK,
                lambdaMult,
                cancellationToken)
            .ConfigureAwait(false);

        return documents;
    }

    /// <inheritdoc />
    protected override Func<float, float> SelectRelevanceScoreFn()
    {
        if (OverrideRelevanceScoreFn != null)
        {
            return OverrideRelevanceScoreFn;
        }

        return GetFuncForDistance();

        Func<float, float> GetFuncForDistance()
        {
            return distance =>
            {
                const string distanceKey = "hnsw:space";

                var distanceType = "l2";

                if (CollectionMetadata != null
                    && CollectionMetadata.TryGetValue(distanceKey, out var value))
                {
                    distanceType = value;
                }

                return distanceType switch
                {
                    "cosine" => CosineRelevanceScoreFn(distance),
                    "l2" => EuclideanRelevanceScoreFn(distance),
                    "ip" => MaxInnerProductRelevanceScoreFn(distance),

                    _ => throw new ArgumentException(
                        $@"No supported normalization function for distance metric of type: {distanceType}.
                        Consider providing relevance_score_fn to Chroma constructor.")
                };
            };
        }
    }

    private async Task<IEnumerable<string>> AddCoreAsync(
        string[] texts,
        Dictionary<string, object>[] metadatas,
        string[] ids,
        CancellationToken cancellationToken)
    {
        float[][] embeddings = await EmbeddingModel
            .CreateEmbeddingsAsync(texts, null, cancellationToken)
            .ConfigureAwait(false);

        var records = new MemoryRecord[texts.Length];
        for (var index = 0; index < texts.Length; index++)
        {
            // TODO: check: description, externalSourceName, key
            records[index] = new MemoryRecord
            (
                new MemoryRecordMetadata
                (
                    isReference: false,
                    id: ids[index],
                    text: texts[index],
                    description: string.Empty,
                    externalSourceName: string.Empty,
                    additionalMetadata: SerializeMetadata(metadatas[index])
                ),
                new Embedding<float>(embeddings[index]),
                key: null
            );
        }

        var resultIds = new List<string>(texts.Length);
        var resultIdsIterator = _store.UpsertBatchAsync(_collectionName, records, cancellationToken);
        await foreach (var item in resultIdsIterator.ConfigureAwait(false))
        {
            resultIds.Add(item);
        }

        return resultIds;
    }

    private async Task<IEnumerable<(Document, float)>> SimilaritySearchByVectorWithAsync(
        float[] embeddings,
        int k,
        CancellationToken cancellationToken)
    {
        var matches = await _store
            .GetNearestMatchesAsync(
                _collectionName,
                new Embedding<float>(embeddings),
                k,
                cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return matches.Select(
            record =>
            {
                var text = record.Item1.Metadata.Text;
                var metadata = DeserializeMetadata(record.Item1.Metadata);

                return (new Document(text, metadata), (float)record.Item2);
            });
    }

    private string SerializeMetadata(Dictionary<string, object> metadata)
    {
        return JsonSerializer.Serialize(metadata, _jsonSerializerOptions);
    }

    private Dictionary<string, object> DeserializeMetadata(MemoryRecordMetadata metadata)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(metadata.AdditionalMetadata, _jsonSerializerOptions)
               ?? new Dictionary<string, object>();
    }
}
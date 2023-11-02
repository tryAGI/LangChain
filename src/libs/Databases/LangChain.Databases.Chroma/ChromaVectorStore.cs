using LangChain.Docstore;
using LangChain.VectorStores;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.Memory.Chroma;
using Microsoft.SemanticKernel.Memory;

namespace LangChain.Databases;

/// <summary>
/// ChromaDB vector store.
/// <see cref="https://api.python.langchain.com/en/latest/_modules/langchain/vectorstores/chroma.html"/>
/// </summary>
public class ChromaVectorStore : VectorStore
{
    private const string LangchainDefaultCollectionName = "langchain";

    // TODO: SemanticKernel impl doesn't support collection metadata. Need changes when moved to another impl
    private Dictionary<string, string>? CollectionMetadata { get; } = new();

    private readonly ChromaMemoryStore _store;

    /// <summary>
    /// Collection name
    /// </summary>
    public string CollectionName { get; init; } = LangchainDefaultCollectionName;

    /// <inheritdoc />
    public ChromaVectorStore(HttpClient httpClient, string endpoint)
    {
        var client = new ChromaClient(httpClient, endpoint);

        client.CreateCollectionAsync(CollectionName).GetAwaiter().GetResult();
        
        _store = new ChromaMemoryStore(client);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddDocumentsAsync(
        IEnumerable<Document> documents,
        CancellationToken cancellationToken = default)
    {
        var documentsArray = documents.ToArray();
        var texts = new string[documentsArray.Length];
        var metadatas = new Dictionary<string, object>[documentsArray.Length];
        for (var index = 0; index < documentsArray.Length; index++)
        {
            var pageContent = documentsArray[index].PageContent;
            texts[index] = pageContent;

            var metadata = documentsArray[index].Metadata;
            metadata["text"] = pageContent;
            if (!metadata.ContainsKey("id"))
            {
                metadata["id"] = Guid.NewGuid().ToString();
            }

            metadatas[index] = metadata;
        }

        var ids = await AddCoreAsync(texts, metadatas, cancellationToken).ConfigureAwait(false);

        return ids;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddTextsAsync(
        IEnumerable<string> texts,
        IEnumerable<Dictionary<string, object>> metadatas = null,
        CancellationToken cancellationToken = default)
    {
        var ids = await AddCoreAsync(
                texts.ToArray(),
                metadatas.ToArray(),
                cancellationToken)
            .ConfigureAwait(false);

        return ids;
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        await _store.RemoveBatchAsync(CollectionName, ids, cancellationToken).ConfigureAwait(false);

        return true;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> SimilaritySearchAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await Embeddings
            .EmbedQueryAsync(query, cancellationToken)
            .ConfigureAwait(false);

        var documentsWithScores = await SimilaritySearchWithVectorCoreAsync(embeddings, k, cancellationToken).ConfigureAwait(false);
        var documents = documentsWithScores.Select(dws => dws.Item1);

        return documents;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(
        IEnumerable<float> embedding,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var documentsWithScores = await SimilaritySearchWithVectorCoreAsync(embedding.ToArray(), k, cancellationToken).ConfigureAwait(false);
        var documents = documentsWithScores.Select(dws => dws.Item1);

        return documents;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await Embeddings
            .EmbedQueryAsync(query, cancellationToken)
            .ConfigureAwait(false);

        var documentsWithScores = await SimilaritySearchWithVectorCoreAsync(embeddings, k, cancellationToken).ConfigureAwait(false);

        return documentsWithScores;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(
        IEnumerable<float> embedding,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Querying not supported by SemanticKernel impl.");

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(
        string query,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await Embeddings
            .EmbedQueryAsync(query, cancellationToken)
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
        CancellationToken cancellationToken)
    {
        var embeddings = await Embeddings
            .EmbedDocumentsAsync(texts, cancellationToken)
            .ConfigureAwait(false);

        var records = new MemoryRecord[texts.Length];
        for (var index = 0; index < texts.Length; index++)
        {
            // TODO: check: description, externalSourceName, additionalMetadata, key
            records[index] = new MemoryRecord
            (
                new MemoryRecordMetadata
                (
                    isReference: false,
                    id: metadatas[index]["id"].ToString(),
                    text: texts[index],
                    description: string.Empty,
                    externalSourceName: string.Empty,
                    additionalMetadata: string.Empty
                ),
                new Embedding<float>(embeddings[index]),
                key: null
            );
        }

        var ids = new List<string>(texts.Length);
        var idsIterator = _store.UpsertBatchAsync(CollectionName, records, cancellationToken);
        await foreach (var item in idsIterator.ConfigureAwait(false))
        {
            ids.Add(item);
        }

        return ids;
    }

    private async Task<IEnumerable<(Document, float)>> SimilaritySearchWithVectorCoreAsync(
        float[] embeddings,
        int k,
        CancellationToken cancellationToken)
    {
        var matches = await _store
            .GetNearestMatchesAsync(
                CollectionName,
                new Embedding<float>(embeddings),
                k,
                cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        // TODO: extract metadata from MemoryRecord.Metadata.AdditionalMetadata
        return matches.Select(
            record =>
                (new Document(record.Item1.Metadata.Text, new Dictionary<string, object>()), (float)record.Item2));
    }
}
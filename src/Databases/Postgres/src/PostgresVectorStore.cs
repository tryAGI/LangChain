using System.Diagnostics.CodeAnalysis;
using LangChain.Databases.Postgres;
using LangChain.Sources;
using LangChain.Providers;
using LangChain.VectorStores;

namespace LangChain.Databases;

/// <summary>
/// Postgres vector store (using <see href="https://github.com/pgvector/pgvector"/>)
///
/// <remarks>
/// required: CREATE EXTENSION IF NOT EXISTS vector
/// </remarks>
/// </summary>
[RequiresDynamicCode("Requires dynamic code.")]
[RequiresUnreferencedCode("Requires unreferenced code.")]
public class PostgresVectorStore : VectorStore
{
    private readonly DistanceStrategy _distanceStrategy;
    private readonly string _collectionName;
    private const string DefaultSchema = "public";
    private const string DefaultCollectionName = "langchain";

    private readonly PostgresDbClient _postgresDbClient;

    /// <inheritdoc />
    public PostgresVectorStore(
        string connectionString,
        int vectorSize,
        IEmbeddingModel embeddingModel,
        string schema = DefaultSchema,
        string collectionName = DefaultCollectionName,
        DistanceStrategy distanceStrategy = DistanceStrategy.Cosine,
        Func<float, float>? overrideRelevanceScoreFn = null)
        : base(embeddingModel, overrideRelevanceScoreFn)
    {
        _distanceStrategy = distanceStrategy;
        _collectionName = collectionName;

        _postgresDbClient = new PostgresDbClient(connectionString, schema, vectorSize);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddDocumentsAsync(
        IEnumerable<Document> documents,
        CancellationToken cancellationToken = default)
    {
        var documentsArray = documents.ToArray();
        float[][] embeddings = await EmbeddingModel
            .CreateEmbeddingsAsync(documentsArray
                .Select(d => d.PageContent)
                .ToArray(), null, cancellationToken)
            .ConfigureAwait(false);

        var ids = new string[documentsArray.Length];
        foreach (var (document, i) in documentsArray.Select((d, i) => (d, i)))
        {
            ids[i] = Guid.NewGuid().ToString();
            await _postgresDbClient.UpsertAsync(
                _collectionName,
                id: ids[i],
                document.PageContent,
                document.Metadata,
                embeddings[i],
                DateTime.UtcNow,
                cancellationToken
                ).ConfigureAwait(false);
        }

        return ids;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddTextsAsync(
        IEnumerable<string> texts,
        IEnumerable<Dictionary<string, object>>? metadatas = null,
        CancellationToken cancellationToken = default)
    {
        var textsArray = texts.ToArray();
        var metadatasArray = metadatas?.ToArray() ?? new Dictionary<string, object>?[textsArray.Length];
        
        float[][] embeddings = await EmbeddingModel
            .CreateEmbeddingsAsync(textsArray, null, cancellationToken)
            .ConfigureAwait(false);

        var ids = new string[textsArray.Length];
        for (var i = 0; i < textsArray.Length; i++)
        {
            ids[i] = Guid.NewGuid().ToString();
            await _postgresDbClient.UpsertAsync(
                _collectionName,
                id: ids[i],
                textsArray[i],
                metadatasArray[i],
                embeddings[i],
                DateTime.UtcNow,
                cancellationToken
            ).ConfigureAwait(false);
        }

        return ids;
    }

    /// <summary>
    /// Get document by id
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="cancellationToken"></param>
    public async Task<Document?> GetDocumentByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var record = await _postgresDbClient.GetRecordByIdAsync(_collectionName, id, withEmbeddings: false, cancellationToken).ConfigureAwait(false);

        return record != null
            ? new Document(record.Content, record.Metadata)
            : null;
    }
    
    /// <inheritdoc />
    public override async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        await _postgresDbClient
            .DeleteBatchAsync(_collectionName, ids.ToList(), cancellationToken)
            .ConfigureAwait(false);

        return true;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(
        IEnumerable<float> embedding,
        int k = 4, CancellationToken cancellationToken = default)
    {
        var records = await SimilaritySearchByVectorWithScoreAsync(embedding, k, cancellationToken)
            .ConfigureAwait(false);

        return records.Select(r => r.Item1);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(
        string query, int k = 4,
        CancellationToken cancellationToken = default)
    {
        float[] embedding = await EmbeddingModel.CreateEmbeddingsAsync(
            query, null, cancellationToken).ConfigureAwait(false);

        return await SimilaritySearchByVectorWithScoreAsync(
                embedding, k, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(
        IEnumerable<float> embedding,
        int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        return Task.FromException<IEnumerable<Document>>(new NotSupportedException("Querying not supported by SemanticKernel impl."));

        // TODO: implement maximal_marginal_relevance method or call python?
        // var embeddingArray = embedding.ToArray();
        // var results = await _postgresDbClient
        //     .GetWithDistanceAsync(
        //         _collectionName,
        //         embeddingArray,
        //         _distanceStrategy,
        //         limit: k,
        //         withEmbeddings: true,
        //         cancellationToken: cancellationToken)
        //     .ConfigureAwait(false);
        //
        // var mmrSelected = maximal_marginal_relevance(
        //     embeddingArray,
        //     results.Select(r => r.Item1.Embedding!),
        //     k: k,
        //     lambdaMult: lambdaMult);
        //
        // var resultDocs = results
        //     .Select((r, i) => (Record: r.Item1, Index: i))
        //     .Where(v => mmrSelected.Contains(v.Index))
        //     .Select(v => new Document(v.Record.Content, v.Record.Metadata));
        //
        // return resultDocs;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(
        string query, int k = 4,
        int fetchK = 20, float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        float[] embedding = await EmbeddingModel.CreateEmbeddingsAsync(query, null, cancellationToken).ConfigureAwait(false);

        return await MaxMarginalRelevanceSearchByVector(embedding, k, fetchK, lambdaMult, cancellationToken)
            .ConfigureAwait(false);
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
                return _distanceStrategy switch
                {
                    DistanceStrategy.Euclidean => EuclideanRelevanceScoreFn(distance),
                    DistanceStrategy.Cosine => CosineRelevanceScoreFn(distance),
                    DistanceStrategy.InnerProduct => MaxInnerProductRelevanceScoreFn(distance),
                    _ => throw new ArgumentOutOfRangeException(
                        $"No supported normalization function for {nameof(_distanceStrategy)} of {_distanceStrategy}." +
                        $"Consider providing nameof(overrideRelevanceScoreFn) to constructor.")
                };
            };
        }
    }

    private async Task<IEnumerable<(Document, float)>> SimilaritySearchByVectorWithScoreAsync(
        IEnumerable<float> embedding, int k = 4,
        CancellationToken cancellationToken = default)
    {
        var records = await _postgresDbClient
            .GetWithDistanceAsync(
                _collectionName,
                embedding.ToArray(),
                _distanceStrategy,
                limit: k,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return records.Select(r => (new Document(r.Item1.Content, r.Item1.Metadata), r.Item2));
    }
}
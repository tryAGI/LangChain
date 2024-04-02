using System.Diagnostics.CodeAnalysis;
using LangChain.Databases.Postgres;
using LangChain.Sources;

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
        string schema = DefaultSchema,
        string collectionName = DefaultCollectionName,
        DistanceStrategy distanceStrategy = DistanceStrategy.Cosine,
        Func<float, float>? overrideRelevanceScoreFn = null)
        : base(overrideRelevanceScoreFn)
    {
        _distanceStrategy = distanceStrategy;
        _collectionName = collectionName;

        _postgresDbClient = new PostgresDbClient(connectionString, schema, vectorSize);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddTextsAsync(
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IReadOnlyDictionary<string, object>>? metadatas = null,
        IReadOnlyCollection<float[]>? embeddings = null,
        CancellationToken cancellationToken = default)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));
        
        var ids = new string[texts.Count];
        for (var i = 0; i < texts.Count; i++)
        {
            ids[i] = Guid.NewGuid().ToString();
            await _postgresDbClient.UpsertAsync(
                _collectionName,
                id: ids[i],
                texts.ElementAt(i),
                metadatas?.ElementAt(i),
                embeddings?.ElementAt(i),
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
    public override Func<float, float> SelectRelevanceScoreFn()
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

    public override async Task<IEnumerable<(string Text, Dictionary<string, object>? Metadata, float Distance)>> SimilaritySearchByVectorWithScoreAsync(
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

        return records.Select(r => (r.Item1.Content, r.Item1.Metadata, r.Item2));
    }
}
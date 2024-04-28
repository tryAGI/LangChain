using System.Diagnostics.CodeAnalysis;
using LangChain.Sources;

namespace LangChain.Databases.Postgres;

/// <summary>
/// Postgres vector store (using <see href="https://github.com/pgvector/pgvector"/>)
/// <remarks>
/// required: CREATE EXTENSION IF NOT EXISTS vector
/// </remarks>
/// </summary>
[RequiresDynamicCode("Requires dynamic code.")]
[RequiresUnreferencedCode("Requires unreferenced code.")]
public class PostgresVectorDatabase(
    string connectionString,
    int vectorSize,
    string schema = PostgresVectorDatabase.DefaultSchema,
    string collectionName = PostgresVectorDatabase.DefaultCollectionName)
    : IVectorDatabase
{
    private const string DefaultSchema = "public";
    private const string DefaultCollectionName = "langchain";

    private readonly PostgresDbClient _postgresDbClient = new(connectionString, schema, vectorSize);

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<Vector> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));
        
        foreach (var item in items)
        {
            await _postgresDbClient.UpsertAsync(
                tableName: collectionName,
                id: item.Id,
                content: item.Text,
                metadata: item.Metadata,
                embedding: item.Embedding,
                timestamp: DateTime.UtcNow,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }
        
        return items
            .Select(i => i.Id)
            .ToArray();
    }

    /// <summary>
    /// Get document by id
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="cancellationToken"></param>
    public async Task<Document?> GetDocumentByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var record = await _postgresDbClient.GetRecordByIdAsync(collectionName, id, withEmbeddings: false, cancellationToken).ConfigureAwait(false);

        return record != null
            ? new Document(record.Content, record.Metadata)
            : null;
    }
    
    /// <inheritdoc />
    public async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        await _postgresDbClient
            .DeleteBatchAsync(collectionName, ids.ToList(), cancellationToken)
            .ConfigureAwait(false);

        return true;
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        settings ??= new VectorSearchSettings();
        
        var records = await _postgresDbClient
            .GetWithDistanceAsync(
                collectionName,
                request.Embeddings.First(),
                settings.DistanceStrategy,
                limit: settings.NumberOfResults,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        // MMR
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

        return new VectorSearchResponse
        {
            Items = records
                .Select(r => new Vector
                {
                    Text = r.Item1.Content,
                    Metadata = r.Item1.Metadata,
                    Distance = r.Item2,
                })
                .ToArray(),   
        };
    }
}
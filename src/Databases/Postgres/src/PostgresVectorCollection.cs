namespace LangChain.Databases.Postgres;

/// <summary>
/// Postgres vector store (using <see href="https://github.com/pgvector/pgvector"/>)
/// <remarks>
/// required: CREATE EXTENSION IF NOT EXISTS vector
/// </remarks>
/// </summary>
public class PostgresVectorCollection(
    PostgresDbClient client,
    string name = VectorCollection.DefaultName,
    string? id = null)
    : VectorCollection(name, id), IVectorCollection
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<Vector> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            await client.UpsertAsync(
                tableName: Name,
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

    /// <inheritdoc />
    public async Task<Vector?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var record = await client.GetRecordByIdAsync(Name, id, withEmbeddings: false, cancellationToken).ConfigureAwait(false);

        return record != null
            ? new Vector
            {
                Text = record.Content,
                Metadata = record.Metadata,
            }
            : null;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        await client
            .DeleteBatchAsync(Name, ids.ToList(), cancellationToken)
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

        var records = await client
            .GetWithDistanceAsync(
                Name,
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

    /// <inheritdoc />
    public Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
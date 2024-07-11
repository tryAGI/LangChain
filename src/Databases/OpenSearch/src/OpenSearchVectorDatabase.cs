using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch;

/// <summary>
/// Represents a vector database using OpenSearch.
/// </summary>
public class OpenSearchVectorDatabase : IVectorDatabase
{
    private readonly OpenSearchClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchVectorDatabase"/> class.
    /// </summary>
    /// <param name="options">The OpenSearch vector store options.</param>
    public OpenSearchVectorDatabase(OpenSearchVectorDatabaseOptions? options = null)
    {
        options ??= new OpenSearchVectorDatabaseOptions();

#pragma warning disable CA2000
        var settings = new ConnectionSettings(options.ConnectionUri)
#pragma warning restore CA2000
            .BasicAuthentication(options.Username, options.Password);

        _client = new OpenSearchClient(settings);
    }

    /// <inheritdoc />
    public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        var response = await _client.Indices.CreateAsync(collectionName, c => c
            .Settings(x => x
                .Setting("index.knn", true)
                .Setting("index.knn.space_type", "cosinesimil")
            )
            .Map<VectorRecord>(m => m
                .Properties(p => p
                    .Keyword(k => k.Name(n => n.Id))
                    .Nested<Dictionary<string, object>>(n => n.Name(x => x.Metadata))
                    .Text(t => t.Name(n => n.Text))
                    .KnnVector(d => d.Name(n => n.Vector).Dimension(dimensions).Similarity("cosine"))
                )
            ), cancellationToken).ConfigureAwait(false);
        if (!response.IsValid)
        {
            throw new InvalidOperationException($"Failed to create collection '{collectionName}'. DebugInformation: {response.DebugInformation}");
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.Indices.GetAsync(Indices.AllIndices, ct: cancellationToken).ConfigureAwait(false);

        return response.Indices.Keys
            .Select(x => x.Name)
            .Where(x => !x.StartsWith(".", StringComparison.Ordinal))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var response = await _client.Indices.GetAsync(
            new GetIndexRequest(collectionName), cancellationToken).ConfigureAwait(false);
        if (!response.IsValid)
        {
            throw new InvalidOperationException($"Collection not found. DebugInformation: {response.DebugInformation}");
        }

        return new OpenSearchVectorCollection(
            client: _client,
            name: collectionName,
            id: collectionName);
    }

    /// <inheritdoc />
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var response = await _client.Indices.DeleteAsync(new DeleteIndexRequest(collectionName), cancellationToken).ConfigureAwait(false);
        if (!response.IsValid)
        {
            throw new InvalidOperationException($"Failed to delete collection '{collectionName}'. DebugInformation: {response.DebugInformation}");
        }
    }

    /// <inheritdoc />
    public async Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            await CreateCollectionAsync(collectionName, dimensions, cancellationToken).ConfigureAwait(false);
        }

        return await GetCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var response = await _client.Indices.ExistsAsync(collectionName, ct: cancellationToken).ConfigureAwait(false);

        return response.Exists;
    }
}
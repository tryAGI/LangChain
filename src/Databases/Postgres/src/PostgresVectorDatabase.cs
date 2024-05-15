using System.Diagnostics.CodeAnalysis;

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
    string schema = PostgresVectorDatabase.DefaultSchema)
    : IVectorDatabase
{
    private const string DefaultSchema = "public";

    private readonly PostgresDbClient _client = new(connectionString, schema);

    /// <inheritdoc />
    public async Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException($"Collection '{collectionName}' does not exist.");
        }

        return new PostgresVectorCollection(_client, collectionName);
    }

    /// <inheritdoc />
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        await _client.DropTableAsync(collectionName, cancellationToken).ConfigureAwait(false);
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
        return await _client.IsTableExistsAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        await _client.CreateEmbeddingTableAsync(
            tableName: collectionName,
            dimensions: dimensions,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        return _client.ListTablesAsync(cancellationToken);
    }
}
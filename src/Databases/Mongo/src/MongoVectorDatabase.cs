using LangChain.Databases.Mongo.Client;
using MongoDB.Driver;

namespace LangChain.Databases.Mongo;

public class MongoVectorDatabase(
    string connectionString,
    string schema = MongoVectorDatabase.DefaultSchema)
    : IVectorDatabase
{
    private const string DefaultSchema = "langchain";

    private readonly MongoDbClient _client = new(
        new MongoContext(
            new DatabaseConfiguration
            {
                ConnectionString = connectionString,
                DatabaseName = schema,
            }));

    /// <inheritdoc />
    public async Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException($"Collection '{collectionName}' does not exist.");
        }

        var context = new MongoContext(new DatabaseConfiguration
        {
            ConnectionString = connectionString,
            DatabaseName = schema,
        });

        return new MongoVectorCollection(context, "idx_" + collectionName, name: collectionName);
    }

    /// <inheritdoc />
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        await _client.DropCollectionAsync(collectionName).ConfigureAwait(false);
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
        return await _client.CollectionExistsAsync(collectionName).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        var collection = await _client.CreateCollection<Vector>(collectionName).ConfigureAwait(false);
        var indexName = await collection.Indexes.CreateOneAsync(new CreateIndexModel<Vector>(
            Builders<Vector>.IndexKeys.Ascending(v => v.Embedding)
                .Ascending(v => v.Text), new CreateIndexOptions
                {
                    Background = true,
                }), cancellationToken: cancellationToken).ConfigureAwait(false);
        return;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetCollections().ConfigureAwait(false);
    }
}
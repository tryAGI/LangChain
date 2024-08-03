using Elastic.Clients.Elasticsearch;

namespace LangChain.Databases.Elasticsearch;

/// <summary>
/// Elasticsearch vector store.
/// </summary>
public class ElasticsearchVectorDatabase(
    ElasticsearchClient client)
    : IVectorDatabase
{
    /// <inheritdoc />
    public async Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        // try
        // {
        //     // var collection = await client.GetCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Collection not found");
        //     //
        //     // return new ElasticsearchVectorCollection(
        //     //     client,
        //     //     name: collection.Name,
        //     //     id: collection.Id);
        // }
        // catch (Exception exception)
        // {
        //     throw new InvalidOperationException("Collection not found", innerException: exception);
        // }
    }

    /// <inheritdoc />
    public async Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        // await foreach (var name in client.ListCollectionsAsync(cancellationToken).ConfigureAwait(false))
        // {
        //     if (name == collectionName)
        //     {
        //         return true;
        //     }
        // }
        //
        // return false;
    }

    /// <inheritdoc />
    public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        //await client.CreateCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        //return await client.ListCollectionsAsync(cancellationToken).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        // if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        // {
        //     await client.CreateCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
        // }
        //
        // return await GetCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        //await client.DeleteCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }
}
using Microsoft.SemanticKernel.Memory;

namespace LangChain.Databases.SemanticKernel;

public class SemanticKernelMemoryDatabase(IMemoryStore store) : IVectorDatabase
{
    public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        await store.CreateCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        await store.DeleteCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        var collections = await ListCollectionsAsync(cancellationToken).ConfigureAwait(false);
        var collection = collections.FirstOrDefault(x => x == collectionName);
        return collection != null ? new SemanticKernelMemoryStoreCollection(store, collection)
            : throw new InvalidOperationException("Collection not found");
    }

    public async Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
            await store.CreateCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
        return new SemanticKernelMemoryStoreCollection(store, collectionName);
    }

    public async Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        return await store.DoesCollectionExistAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var collections = store.GetCollectionsAsync(cancellationToken);
        return await collections.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}
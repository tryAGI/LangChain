using System.Collections.Concurrent;

namespace LangChain.Databases.InMemory;

/// <inheritdoc />
public class InMemoryVectorDatabase : IVectorDatabase
{
    private readonly ConcurrentDictionary<string, IVectorCollection> _collections = [];

    public Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        return _collections.TryGetValue(collectionName, out var collection)
            ? Task.FromResult(collection)
            : Task.FromException<IVectorCollection>(new InvalidOperationException("Collection not found"));
    }

    public Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        _collections.TryRemove(collectionName, out _);
        return Task.CompletedTask;
    }

    public Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_collections.GetOrAdd(collectionName, _ => new InMemoryVectorCollection(collectionName)));
    }

    public Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_collections.ContainsKey(collectionName));
    }
}
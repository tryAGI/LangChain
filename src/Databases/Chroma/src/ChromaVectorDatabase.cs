using Microsoft.SemanticKernel.Connectors.Chroma;

namespace LangChain.Databases.Chroma;

/// <summary>
/// ChromaDB vector store.
/// According: https://api.python.langchain.com/en/latest/_modules/langchain/vectorstores/chroma.html
/// </summary>
public class ChromaVectorDatabase : IVectorDatabase
{
    private readonly ChromaMemoryStore _store;
    private readonly ChromaClient _client;

    public ChromaVectorDatabase(
        HttpClient httpClient,
        string endpoint)
    {
        _client = new ChromaClient(httpClient, endpoint);
        _store = new ChromaMemoryStore(_client);
    }

    /// <inheritdoc />
    public async Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var collection = await _client.GetCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Collection not found");

            return new ChromaVectorCollection(
                _store,
                name: collection.Name,
                id: collection.Id);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("Collection not found", innerException: exception);
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        await foreach (var name in _client.ListCollectionsAsync(cancellationToken).ConfigureAwait(false))
        {
            if (name == collectionName)
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public async Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        await _client.CreateCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        return await _client.ListCollectionsAsync(cancellationToken).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            await _client.CreateCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
        }

        return await GetCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        await _store.DeleteCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }
}
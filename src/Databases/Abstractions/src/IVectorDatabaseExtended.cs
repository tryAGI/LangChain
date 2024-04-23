namespace LangChain.Databases;

/// <inheritdoc />
public interface IVectorDatabaseExtended : IVectorDatabase
{
    /// <summary>
    /// Get collection
    /// </summary>
    Task<VectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete collection
    /// </summary>
    Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get or create collection
    /// </summary>
    Task<VectorCollection> GetOrCreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if collection exists
    /// </summary>
    Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get collection
    /// </summary>
    Task<VectorSearchItem?> GetItemByIdAsync(string collectionName, string id, CancellationToken cancellationToken = default);
}
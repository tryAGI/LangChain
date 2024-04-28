namespace LangChain.Databases;

/// <summary>
/// Provides access to the vector database. <br/>
/// It's equivalent to the VectorStore in the Python API.
/// </summary>
public interface IVectorDatabase
{
    /// <summary>
    /// Get collection
    /// </summary>
    Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete collection
    /// </summary>
    Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get or create collection
    /// </summary>
    Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if collection exists
    /// </summary>
    Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default);
}
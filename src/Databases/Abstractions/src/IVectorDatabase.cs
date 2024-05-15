namespace LangChain.Databases;

/// <summary>
/// Provides access to the vector database. <br/>
/// It's equivalent to the VectorStore in the Python API.
/// </summary>
public interface IVectorDatabase
{
    /// <summary>
    /// Gets a collection by its name.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the vector collection.</returns>
    Task<IVectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a collection by its name.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a collection by its name.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="dimensions">The number of dimensions of the vectors.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the vector collection.</returns>
    Task<IVectorCollection> GetOrCreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a collection exists.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether the collection exists.</returns>
    Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new collection with the specified options.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="dimensions">The number of dimensions of the vectors.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateCollectionAsync(string collectionName, int dimensions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all collections.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of collection names.</returns>
    Task<IReadOnlyList<string>> ListCollectionsAsync(CancellationToken cancellationToken = default);
}
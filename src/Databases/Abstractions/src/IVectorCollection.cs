namespace LangChain.Databases;

/// <summary>
/// Collection of vectors. It's equivalent to the VectorIndex in the Python API.
/// </summary>
public interface IVectorCollection
{
    /// <summary>
    /// Id of the collection.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Name of the collection.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Adds a collection of vectors to the store.
    /// </summary>
    /// <param name="items">The collection of vector search items to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection of the added item IDs.</returns>
    Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<Vector> items,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an item by its ID from a specific collection.
    /// </summary>
    /// <param name="id">The ID of the item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the vector search item.</returns>
    Task<Vector?> GetAsync(
        string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes items from the store by their IDs.
    /// </summary>
    /// <param name="ids">The IDs of the items to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for vector records based on a search request.
    /// </summary>
    /// <param name="request">The search request.</param>
    /// <param name="settings">The search settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the search response.</returns>
    Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the collection is empty.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default);
}
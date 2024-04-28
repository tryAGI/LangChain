namespace LangChain.Databases;

/// <summary>
/// Collection of vectors. It's equivalent to the VectorIndex in the Python API.
/// </summary>
public interface IVectorCollection
{
    /// <summary>
    /// Id of the collection.
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// Name of the collection.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Adds texts to the vector database and returns the ids.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<Vector> items,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get vector by id.
    /// </summary>
    Task<Vector?> GetAsync(
        string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes by ids.
    /// </summary>
    /// <param name="ids">List of Ids to delete</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a search.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="settings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of documents.</returns>
    public Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default);
}
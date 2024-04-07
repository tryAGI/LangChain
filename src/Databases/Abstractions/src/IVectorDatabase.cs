namespace LangChain.Databases;

/// <summary>
/// VectorStore. Check https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html
/// </summary>
public interface IVectorDatabase
{
    /// <summary>
    /// Adds texts to the vector database and returns the ids.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<string>> AddAsync(
        IReadOnlyCollection<VectorSearchItem> items,
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
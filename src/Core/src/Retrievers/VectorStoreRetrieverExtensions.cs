using LangChain.Sources;

namespace LangChain.VectorStores;

/// <summary>
/// 
/// </summary>
public static class VectorStoreRetrieverExtensions
{
    /// <summary>
    /// Return vector store as retriever
    /// </summary>
    /// <param name="store">vector store</param>
    /// <param name="searchType">search type</param>
    /// <param name="scoreThreshold">score threshold</param>
    /// <returns></returns>
    public static VectorStoreRetriever AsRetriever(
        this VectorStore store,
        ESearchType searchType = ESearchType.Similarity,
        float? scoreThreshold = null)
    {
        return new VectorStoreRetriever(store, searchType, scoreThreshold);
    }

    /// <summary>
    /// Return vector store as retriever
    /// </summary>
    /// <param name="store">vector store</param>
    /// <param name="query"></param>
    /// <param name="amount"></param>
    /// <param name="searchType">search type</param>
    /// <param name="scoreThreshold">score threshold</param>
    /// <returns></returns>
    public static async Task<IReadOnlyCollection<Document>> GetSimilarDocuments(
        this VectorStore store,
        string query,
        int amount = 4,
        ESearchType searchType = ESearchType.Similarity,
        float? scoreThreshold = null)
    {
        var retriever = store.AsRetriever(searchType, scoreThreshold);
        retriever.K = amount;
        
        return await retriever.GetRelevantDocumentsAsync(query).ConfigureAwait(false);
    }
}
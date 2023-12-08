namespace LangChain.VectorStores;

/// <summary>
/// 
/// </summary>
public static class VectorStoreExtensions
{
    /// <summary>
    /// Return vector store as retriever
    /// </summary>
    /// <param name="store">vector store</param>
    /// <param name="searchType">search type</param>
    /// <param name="scoreThreshold">score threshold</param>
    /// <returns></returns>
    public static VectorStoreRetriever AsRetriever(this VectorStore store, ESearchType searchType = ESearchType.Similarity, float? scoreThreshold = null)
    {
        return new VectorStoreRetriever(store, searchType, scoreThreshold);
    }
}
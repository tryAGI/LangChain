using LangChain.Providers;
using LangChain.Retrievers;

namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public static class VectorStoreRetrieverExtensions
{
    /// <summary>
    /// Return vector collection as retriever
    /// </summary>
    /// <param name="vectorCollection">vector store</param>
    /// <param name="embeddingModel"></param>
    /// <param name="searchType">search type</param>
    /// <param name="scoreThreshold">score threshold</param>
    /// <returns></returns>
    public static VectorStoreRetriever AsRetriever(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        VectorSearchType searchType = VectorSearchType.Similarity,
        float? scoreThreshold = null)
    {
        return new VectorStoreRetriever(vectorCollection, embeddingModel, searchType, scoreThreshold);
    }
}
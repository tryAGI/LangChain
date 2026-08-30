using LangChain.Retrievers;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LangChain.Extensions;

/// <summary>
///
/// </summary>
public static class VectorStoreRetrieverExtensions
{
    /// <summary>
    /// Return vector collection as retriever
    /// </summary>
    public static VectorStoreRetriever AsRetriever(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        return new VectorStoreRetriever(vectorCollection, embeddingGenerator);
    }
}

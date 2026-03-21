using LangChain.Callback;
using LangChain.Extensions;
using LangChain.DocumentLoaders;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LangChain.Retrievers;

/// <summary>
/// Base Retriever class for VectorStore.
/// https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html
/// </summary>
public class VectorStoreRetriever : BaseRetriever
{
    /// <summary>
    ///
    /// </summary>
    public VectorStoreCollection<string, LangChainDocumentRecord> VectorCollection { get; init; }

    /// <summary>
    ///
    /// </summary>
    public int K { get; set; } = 4;

    private IEmbeddingGenerator<string, Embedding<float>> EmbeddingGenerator { get; init; }

    /// <inheritdoc/>
    public VectorStoreRetriever(
        VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        EmbeddingGenerator = embeddingGenerator;
        VectorCollection = vectorCollection;
    }

    /// <inheritdoc/>
    protected override async Task<IEnumerable<Document>> GetRelevantDocumentsCoreAsync(
        string query,
        CallbackManagerForRetrieverRun? runManager = null,
        CancellationToken cancellationToken = default)
    {
        return await VectorCollection.GetSimilarDocuments(
            EmbeddingGenerator,
            query,
            amount: K,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<string>> AddDocumentsAsync(IReadOnlyCollection<Document> documents)
    {
        return VectorCollection.AddDocumentsAsync(EmbeddingGenerator, documents);
    }
}

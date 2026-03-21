using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LangChain.Extensions;

/// <summary>
///
/// </summary>
public static class VectorStoreIndexWrapper
{
    /// <summary>
    ///
    /// </summary>
    public static Task<string?> QueryAsync(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string question,
        BaseCombineDocumentsChain llm,
        string inputKey = "question",
        string outputKey = "output_text",
        CancellationToken cancellationToken = default)
    {
        var chain = new RetrievalQaChain(
            new RetrievalQaChainInput(
                llm,
                vectorCollection.AsRetriever(embeddingGenerator))
            {
                InputKey = inputKey,
                OutputKey = outputKey,
            }
        );

        return chain.RunAsync(question, cancellationToken: cancellationToken);
    }
}

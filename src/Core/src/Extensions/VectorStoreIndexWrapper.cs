using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Databases;
using LangChain.Providers;

namespace LangChain.Extensions;

/// <summary>
/// 
/// </summary>
public static class VectorStoreIndexWrapper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vectorCollection"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="question"></param>
    /// <param name="llm"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<string?> QueryAsync(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        string question,
        BaseCombineDocumentsChain llm,
        string inputKey = "question",
        string outputKey = "output_text",
        CancellationToken cancellationToken = default)
    {
        var chain = new RetrievalQaChain(
            new RetrievalQaChainInput(
                llm,
                vectorCollection.AsRetriever(embeddingModel))
            {
                InputKey = inputKey,
                OutputKey = outputKey,
            }
        );

        return chain.RunAsync(question, cancellationToken);
    }
}
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Databases;
using LangChain.Providers;

namespace LangChain.Indexes;

/// <summary>
/// 
/// </summary>
public static class VectorStoreIndexWrapper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vectorDatabase"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="question"></param>
    /// <param name="llm"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public static Task<string?> QueryAsync(
        this IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        string question,
        BaseCombineDocumentsChain llm,
        string inputKey = "question",
        string outputKey = "output_text")
    {
        var chain = new RetrievalQaChain(
            new RetrievalQaChainInput(
                llm,
                vectorDatabase.AsRetriever(embeddingModel))
            {
                InputKey = inputKey,
                OutputKey = outputKey,
            }
        );
        
        return chain.Run(question);
    }
}
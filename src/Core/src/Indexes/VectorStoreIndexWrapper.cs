using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Databases;

namespace LangChain.Indexes;

/// <summary>
/// 
/// </summary>
/// <param name="vectorStore"></param>
public class VectorStoreIndexWrapper(
    VectorStore vectorStore)
{
    /// <summary>
    /// 
    /// </summary>
    public VectorStore Store { get; } = vectorStore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="question"></param>
    /// <param name="llm"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public Task<string?> QueryAsync(
        string question,
        BaseCombineDocumentsChain llm,
        string inputKey = "question",
        string outputKey = "output_text")
    {
        var chain = new RetrievalQaChain(
            new RetrievalQaChainInput(
                llm,
                Store.AsRetriever())
            {
                InputKey = inputKey,
                OutputKey = outputKey,
            }
        );
        
        return chain.Run(question);
    }
}
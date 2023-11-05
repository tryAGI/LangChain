using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Providers;
using LangChain.VectorStores;

namespace LangChain.Indexes;

public class VectorStoreIndexWrapper
{
    public VectorStore Store { get; }

    public VectorStoreIndexWrapper(VectorStore vectorStore)
    {
        Store = vectorStore;
    }

    public Task<string?> QueryAsync(string question, BaseCombineDocumentsChain llm, string inputKey= "question", string outputKey= "output_text")
    {
        var chain = new RetrievalQaChain(
            new RetrievalQaChainInput(
                llm,
                Store.AsRetreiver())
            {
                InputKey= inputKey,
                OutputKey= outputKey,
            }
        );
        return chain.Run(question);
    }
}
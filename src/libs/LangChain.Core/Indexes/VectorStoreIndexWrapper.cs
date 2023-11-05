using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Providers;
using LangChain.VectorStores;

namespace LangChain.Indexes;

public class VectorStoreIndexWrapper
{
    private readonly VectorStore _vectorStore;

    public VectorStoreIndexWrapper(VectorStore vectorStore)
    {
        _vectorStore = vectorStore;
    }

    public Task<string?> QueryAsync(string question, BaseCombineDocumentsChain llm, string documentsKey= "input_documents", string questionKey="question", string outputKey= "output_text")
    {
        var chain = new RetrievalQaChain(
            new RetrievalQaChainInput(
                llm,
                _vectorStore.AsRetreiver())
            {
                InputKey= questionKey,
                DocumentsKey= documentsKey,
                OutputKey= outputKey,
            }
        );
        return chain.Run(question);
    }
}
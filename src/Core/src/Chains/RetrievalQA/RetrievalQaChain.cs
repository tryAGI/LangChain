using LangChain.Callback;
using LangChain.Sources;
using LangChain.Retrievers;

namespace LangChain.Chains.RetrievalQA;

/// <summary>
/// Chain for question-answering against an index.
/// </summary>
/// <param name="fields"></param>
public class RetrievalQaChain(RetrievalQaChainInput fields) : BaseRetrievalQaChain(fields)
{
    private readonly BaseRetriever _retriever = fields.Retriever;

    /// <inheritdoc/>
    public override string ChainType() => "retrieval_qa";

    /// <inheritdoc/>
    public override async Task<IEnumerable<Document>> GetDocsAsync(string question, CallbackManagerForChainRun runManager)
    {
        runManager = runManager ?? throw new ArgumentNullException(nameof(runManager));
        
        return await _retriever.GetRelevantDocumentsAsync(
            question,
            callbacks: new ManagerCallbacks(runManager.GetChild())).ConfigureAwait(false);
    }
}
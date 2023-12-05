using LangChain.Chains.CombineDocuments;
using LangChain.Retrievers;

namespace LangChain.Chains.RetrievalQA;

/// <summary>
/// 
/// </summary>
/// <param name="combineDocumentsChain"></param>
/// <param name="retriever"></param>
public class RetrievalQaChainInput(
    BaseCombineDocumentsChain combineDocumentsChain,
    BaseRetriever retriever)
    : BaseRetrievalQaChainInput(combineDocumentsChain)
{
    /// <summary> Documents retriever. </summary>
    public BaseRetriever Retriever { get; } = retriever;
}
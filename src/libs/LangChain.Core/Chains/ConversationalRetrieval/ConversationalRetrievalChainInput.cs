using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Retrievers;

namespace LangChain.Chains.ConversationalRetrieval;

public class ConversationalRetrievalChainInput(
        BaseRetriever retriever,
        BaseCombineDocumentsChain combineDocsChain,
        ILlmChain questionGenerator)
    : BaseConversationalRetrievalChainInput(combineDocsChain, questionGenerator)
{
    /// <summary>
    /// Retriever to use to fetch documents.
    /// </summary>
    public BaseRetriever Retriever { get; } = retriever;

    /// <summary>
    /// If set, enforces that the documents returned are less than this limit.
    /// This is only enforced if <see cref="BaseConversationalRetrievalChainInput.CombineDocsChain"/> is of type <see cref="StuffDocumentsChain"/>.
    /// </summary>
    public int? MaxTokensLimit { get; set; }
}
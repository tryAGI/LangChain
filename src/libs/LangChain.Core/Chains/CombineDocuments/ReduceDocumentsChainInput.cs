namespace LangChain.Chains.CombineDocuments;

public class ReduceDocumentsChainInput : BaseCombineDocumentsChainInput
{
    /// <summary>
    /// Final chain to call to combine documents.
    /// This is typically a StuffDocumentsChain.
    /// </summary>
    public BaseCombineDocumentsChain CombineDocumentsChain { get; set; }
    
    /// <summary>
    /// Chain to use to collapse documents if needed until they can all fit.
    /// If null, will use the combine_documents_chain.
    /// This is typically a StuffDocumentsChain.
    /// </summary>
    public BaseCombineDocumentsChain? CollapseDocumentsChain { get; set; }
    
    /// <summary>
    /// The maximum number of tokens to group documents into. For example, if
    /// set to 3000 then documents will be grouped into chunks of no greater than
    /// 3000 tokens before trying to combine them into a smaller chunk.
    /// </summary>
    public int TokenMax { get; set; } = 3_000;
}
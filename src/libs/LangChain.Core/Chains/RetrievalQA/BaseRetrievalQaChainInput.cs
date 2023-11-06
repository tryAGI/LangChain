using LangChain.Base;
using LangChain.Chains.CombineDocuments;

namespace LangChain.Chains.RetrievalQA;

public class BaseRetrievalQaChainInput(BaseCombineDocumentsChain combineDocumentsChain) : ChainInputs
{
    /// <summary> Chain to use to combine the documents. </summary>
    public BaseCombineDocumentsChain CombineDocumentsChain { get; } = combineDocumentsChain;
    
    /// <summary> Return the source documents or not. </summary>
    public bool ReturnSourceDocuments { get; set; }

    public string InputKey { get; set; } = "question";
    public string OutputKey { get; set; } = "output_text";
}
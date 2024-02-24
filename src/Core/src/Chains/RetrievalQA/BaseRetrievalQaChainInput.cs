using LangChain.Base;
using LangChain.Chains.CombineDocuments;

namespace LangChain.Chains.RetrievalQA;

/// <summary>
/// 
/// </summary>
/// <param name="combineDocumentsChain"></param>
public class BaseRetrievalQaChainInput(BaseCombineDocumentsChain combineDocumentsChain) : ChainInputs
{
    /// <summary> Chain to use to combine the documents. </summary>
    public BaseCombineDocumentsChain CombineDocumentsChain { get; } = combineDocumentsChain;

    /// <summary> Return the source documents or not. </summary>
    public bool ReturnSourceDocuments { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string InputKey { get; set; } = "question";
    
    /// <summary>
    /// 
    /// </summary>
    public string OutputKey { get; set; } = "output_text";
}
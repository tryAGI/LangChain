using LangChain.Base;

namespace LangChain.Chains.CombineDocuments;

/// <inheritdoc />
public abstract class BaseCombineDocumentsChainInput : ChainInputs
{
    /// <summary>
    /// 
    /// </summary>
    public string InputKey { get; set; } = "input_documents";
    
    /// <summary>
    /// 
    /// </summary>
    public string OutputKey { get; set; } = "output_text";
}
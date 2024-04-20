using LangChain.Splitters.Text;

namespace LangChain.Chains.CombineDocuments;

/// <inheritdoc/>
public class AnalyzeDocumentsChainInput(
    BaseCombineDocumentsChain combineDocumentsChain)
    : BaseCombineDocumentsChainInput
{
    /// <summary>
    /// 
    /// </summary>
    public BaseCombineDocumentsChain CombineDocumentsChain { get; set; } = combineDocumentsChain;
    
    /// <summary>
    /// 
    /// </summary>
    public ITextSplitter? Splitter { get; set; }
}
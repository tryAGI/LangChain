using LangChain.Base;

namespace LangChain.Chains.CombineDocuments;

public class AnalyzeDocumentsChainInput(BaseCombineDocumentsChain combineDocumentsChain) : BaseCombineDocumentsChainInput
{
    public BaseCombineDocumentsChain CombineDocumentsChain { get; set; } = combineDocumentsChain;
    public TextSplitter? Splitter { get; set; }
}
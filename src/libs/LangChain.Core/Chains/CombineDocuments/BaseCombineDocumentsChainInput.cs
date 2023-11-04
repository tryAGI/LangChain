using LangChain.Base;

namespace LangChain.Chains.CombineDocuments;

/// <inheritdoc />
public abstract class BaseCombineDocumentsChainInput : ChainInputs
{
    public string InputKey { get; set; } = "input_documents";
    public string OutputKey { get; set; } = "output_text";
}
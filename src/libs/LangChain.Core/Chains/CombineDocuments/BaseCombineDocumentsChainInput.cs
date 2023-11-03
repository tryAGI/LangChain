using LangChain.Base;
using LangChain.Callback;

namespace LangChain.Chains.CombineDocuments;

/// <inheritdoc />
public abstract class BaseCombineDocumentsChainInput : IChainInputs
{
    public string InputKey { get; set; } = "input_documents";
    public string OutputKey { get; set; } = "output_text";
    public bool? Verbose { get; set; }
    public CallbackManager? CallbackManager { get; set; }
}
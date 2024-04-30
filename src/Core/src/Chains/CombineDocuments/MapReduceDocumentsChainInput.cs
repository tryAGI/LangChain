using LangChain.Chains.LLM;

namespace LangChain.Chains.CombineDocuments;

/// <inheritdoc/>
public class MapReduceDocumentsChainInput : BaseCombineDocumentsChainInput
{
    /// <summary>
    /// Chain to apply to each document individually.
    /// </summary>
    public required ILlmChain LlmChain { get; set; }

    /// <summary>
    /// Chain to use to reduce the results of applying `llm_chain` to each doc.
    /// This typically either a ReduceDocumentChain or StuffDocumentChain.
    /// </summary>
    public required BaseCombineDocumentsChain ReduceDocumentsChain { get; set; }

    /// <summary>
    /// The variable name in the llm_chain to put the documents in.
    /// If only one variable in the llm_chain, this need not be provided.
    /// </summary>
    /// <returns></returns>
    public string DocumentVariableName { get; set; } = string.Empty;

    /// <summary>
    /// Return the results of the map steps in the output.
    /// </summary>
    public bool ReturnIntermediateSteps { get; set; }
}
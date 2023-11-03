using LangChain.Chains.LLM;
using LangChain.Prompts;
using LangChain.Prompts.Base;

namespace LangChain.Chains.CombineDocuments;

/// <inheritdoc />
public class StuffDocumentsChainInput(ILlmChain llmChain) : BaseCombineDocumentsChainInput
{
    /// <summary>
    /// LLM chain which is called with the formatted document string, along with any other inputs.
    /// </summary>
    public ILlmChain LlmChain { get; } = llmChain;

    /// <summary>
    /// Prompt to use to format each document, gets passed to `format_document`.
    /// </summary>
    public BasePromptTemplate DocumentPrompt { get; set; } = new PromptTemplate(
        new PromptTemplateInput(
            "{page_content}",
            new List<string>(1) { "page_content" }));

    /// <summary>
    /// The variable name in the LlmChain to put the documents in.
    /// If only one variable in the llm_chain, this need not be provided.
    /// </summary>
    public string? DocumentVariableName { get; set; }

    /// <summary>
    /// The string with which to join the formatted documents
    /// </summary>
    public string DocumentSeparator { get; set; } = "\n\n";
}
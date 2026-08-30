using LangChain.Base;
using LangChain.Memory;
using LangChain.Prompts.Base;
using Microsoft.Extensions.AI;

namespace LangChain.Chains.LLM;

/// <summary>
///
/// </summary>
/// <param name="llm"></param>
/// <param name="prompt"></param>
/// <param name="memory"></param>
public class LlmChainInput(
    IChatClient llm,
    BasePromptTemplate prompt,
    BaseMemory? memory = null)
    : ChainInputs, ILlmChainInput
{
    /// <inheritdoc/>
    public BasePromptTemplate Prompt { get; set; } = prompt;

    /// <inheritdoc/>
    public IChatClient Llm { get; set; } = llm;

    /// <inheritdoc/>
    public string OutputKey { get; set; } = "text";

    /// <summary>
    ///
    /// </summary>
    public BaseMemory? Memory { get; set; } = memory;

    /// <summary>
    /// Whether to return only the final parsed result. Defaults to True.
    /// If false, will return a bunch of extra information about the generation.
    /// </summary>
    public bool ReturnFinalOnly { get; set; } = true;
}

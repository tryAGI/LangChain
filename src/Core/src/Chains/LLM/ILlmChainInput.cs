using LangChain.Base;
using LangChain.Prompts.Base;
using Microsoft.Extensions.AI;

namespace LangChain.Chains.LLM;

/// <inheritdoc/>
public interface ILlmChainInput : IChainInputs
{
    /// <summary>
    ///
    /// </summary>
    BasePromptTemplate Prompt { get; }

    /// <summary>
    ///
    /// </summary>
    IChatClient Llm { get; }

    /// <summary>
    ///
    /// </summary>
    string OutputKey { get; set; }
}

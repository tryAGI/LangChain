using LangChain.Base;
using LangChain.Prompts.Base;
using LangChain.Providers;

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
    IChatModel Llm { get; }

    /// <summary>
    /// 
    /// </summary>
    string OutputKey { get; set; }
}
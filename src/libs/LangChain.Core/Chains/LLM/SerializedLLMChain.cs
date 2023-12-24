using LangChain.LLMS;
using LangChain.Prompts;

namespace LangChain.Chains.LLM;

/// <inheritdoc/>
public class SerializedLlmChain : SerializedBaseChain
{
    /// <summary>
    /// 
    /// </summary>
    public required BaseLlm Llm { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public SerializedPromptTemplate Prompt { get; set; } = new();
}
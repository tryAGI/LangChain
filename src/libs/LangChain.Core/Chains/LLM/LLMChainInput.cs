using LangChain.Base;
using LangChain.Memory;
using LangChain.Prompts.Base;
using LangChain.Providers;

namespace LangChain.Chains.LLM;

public class LlmChainInput(
    IChatModel llm,
    BasePromptTemplate prompt,
    BaseMemory? memory = null)
    : ChainInputs, ILlmChainInput
{
    public BasePromptTemplate Prompt { get; set; } = prompt;
    public IChatModel Llm { get; set; } = llm;
    public string OutputKey { get; set; } = "text";
    public BaseMemory? Memory { get; set; } = memory;
}
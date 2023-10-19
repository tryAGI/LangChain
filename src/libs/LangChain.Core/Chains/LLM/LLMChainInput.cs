using LangChain.Callback;
using LangChain.Memory;
using LangChain.Prompts.Base;
using LangChain.Providers;

namespace LangChain.Chains.LLM;

public class LlmChainInput(
    IChatModel llm,
    BasePromptTemplate prompt,
    BaseMemory? memory = null)
    : ILlmChainInput
{
    public BasePromptTemplate Prompt { get; set; } = prompt;
    public IChatModel Llm { get; set; } = llm;
    public string OutputKey { get; set; }
    public bool? Verbose { get; set; }
    public CallbackManager CallbackManager { get; set; }
    public BaseMemory? Memory { get; set; } = memory;
}
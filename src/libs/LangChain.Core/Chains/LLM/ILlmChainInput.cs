using LangChain.Base;
using LangChain.Prompts.Base;
using LangChain.Providers;

namespace LangChain.Chains.LLM;

public interface ILlmChainInput : IChainInputs
{
    BasePromptTemplate Prompt { get; }
    IChatModel Llm { get; }
    string OutputKey { get; set; }
}
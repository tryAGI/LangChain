using LangChain.Base;
using LangChain.Prompts.Base;

namespace LangChain.Chains.LLM;

public interface ILlmChainInput : IChainInputs
{
    BasePromptTemplate Prompt { get; }
    BaseLanguageModel Llm { get; }
    string OutputKey { get; set; }
}
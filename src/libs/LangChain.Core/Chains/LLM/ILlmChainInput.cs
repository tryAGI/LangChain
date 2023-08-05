using LangChain.NET.Base;
using LangChain.NET.Prompts.Base;

namespace LangChain.NET.Chains.LLM;

public interface ILlmChainInput : IChainInputs
{
    BasePromptTemplate Prompt { get; }
    BaseLanguageModel Llm { get; }
    string OutputKey { get; set; }
}
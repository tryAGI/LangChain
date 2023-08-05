using LangChain.NET.LLMS;
using LangChain.NET.Prompts;

namespace LangChain.NET.Chains.LLM;

public class SerializedLlmChain : SerializedBaseChain
{
    public BaseLlm Llm { get; set; }
    
    public SerializedPromptTemplate Prompt { get; set; }
}
using LangChain.LLMS;
using LangChain.Prompts;

namespace LangChain.Chains.LLM;

public class SerializedLlmChain : SerializedBaseChain
{
    public required BaseLlm Llm { get; set; }

    public SerializedPromptTemplate Prompt { get; set; } = new();
}
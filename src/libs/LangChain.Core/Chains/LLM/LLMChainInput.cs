using LangChain.Base;
using LangChain.Callback;
using LangChain.Prompts.Base;
using LangChain.Providers;

namespace LangChain.Chains.LLM;

public class LlmChainInput : ILlmChainInput
{
    public LlmChainInput(IChatModel llm, BasePromptTemplate prompt)
    {
        this.Llm = llm;
        this.Prompt = prompt;
    }

    public BasePromptTemplate Prompt { get; set; }
    public IChatModel Llm { get; set; }
    public string OutputKey { get; set; }
    public bool? Verbose { get; set; }
    public CallbackManager CallbackManager { get; set; }
}
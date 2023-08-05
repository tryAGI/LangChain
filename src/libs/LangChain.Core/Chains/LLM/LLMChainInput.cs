using LangChain.NET.Base;
using LangChain.NET.Callback;
using LangChain.NET.Prompts.Base;

namespace LangChain.NET.Chains.LLM;

public class LlmChainInput: ILlmChainInput
{
    public LlmChainInput(BaseLanguageModel llm, BasePromptTemplate prompt)
    {
        this.Llm = llm;
        this.Prompt = prompt;
    }
    
    public BasePromptTemplate Prompt { get; set; }
    public BaseLanguageModel Llm { get; set; }
    public string OutputKey { get; set; }
    public bool? Verbose { get; set; }
    public CallbackManager CallbackManager { get; set; }
}
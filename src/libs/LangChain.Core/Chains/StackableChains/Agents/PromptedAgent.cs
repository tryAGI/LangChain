using LangChain.Chains.HelperChains;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains.Agents;

/// <summary>
/// 
/// </summary>
public class PromptedAgent : AgentExecutorChain
{
    /// <summary>
    /// 
    /// </summary>
    public const string Template =
        @"{system}
{history}";

    private static BaseStackableChain MakeChain(string name, string system, IChatModel model, string outputKey)
    {
        return Chain.Set(system, "system")
               | Chain.Template(Template)
               | Chain.LLM(model,outputKey: outputKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="prompt"></param>
    /// <param name="model"></param>
    /// <param name="outputKey"></param>
    public PromptedAgent(
        string name,
        string prompt,
        IChatModel model,
        string outputKey = "final_answer")
        : base(MakeChain(name,prompt,model, outputKey), name, "history", outputKey)
    {
        
    }
}
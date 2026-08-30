using LangChain.Chains.HelperChains;
using Microsoft.Extensions.AI;

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

    private static BaseStackableChain MakeChain(string name, string system, IChatClient chatClient, string outputKey)
    {
        return Chain.Set(system, "system")
               | Chain.Template(Template)
               | Chain.LLM(chatClient, outputKey: outputKey);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="prompt"></param>
    /// <param name="chatClient"></param>
    /// <param name="outputKey"></param>
    public PromptedAgent(
        string name,
        string prompt,
        IChatClient chatClient,
        string outputKey = "final_answer")
        : base(MakeChain(name, prompt, chatClient, outputKey), name, "history", outputKey)
    {

    }
}

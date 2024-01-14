using LangChain.Abstractions.Chains.Base;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;
using LangChain.Chains.StackableChains.ReAct;

namespace LangChain.Chains.StackableChains.Agents.Crew;

public class AgentTask(CrewAgent agent, string description, List<CrewAgentTool>? tools=null)
{
    public CrewAgent Agent { get; set; } = agent;
    public List<CrewAgentTool> Tools { get; set; } = tools??new List<CrewAgentTool>();
    public string Description { get; set; } = description;

    public string Execute(string context=null)
    {
        Agent.AddTools(Tools);
        Agent.Context = context;
        var chain = Chain.Set(Description, "task") 
                    | Agent;
        var res = chain.Run("result").Result;
        return res;
    }
}
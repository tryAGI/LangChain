using System.Security.AccessControl;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;

namespace LangChain.Chains.StackableChains.Agents.Crew;

public class CrewChain(IEnumerable<CrewAgent> allAgents, CrewAgent manager, string inputKey="text", string outputKey="text") : BaseStackableChain
{
    public string? Context { get; set; }

    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        

        var taskText = values.Value[inputKey].ToString();
        
        var task = new AgentTask(manager, taskText);

        // add delegation tools
        if (allAgents.Count()>1)
        {
            task.Tools.Add(new AskQuestionTool(allAgents.Except(new[] { task.Agent })));
            task.Tools.Add(new DelegateWorkTool(allAgents.Except(new[] { task.Agent })));
        }
        
        var res = task.Execute(Context);
        Context = res;

        values.Value[outputKey] = res;
        return Task.FromResult(values);
    }
}
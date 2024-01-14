using LangChain.Chains.StackableChains.Agents.Crew.Tools;

namespace LangChain.Chains.StackableChains.Agents.Crew;

public class Crew(IEnumerable<CrewAgent> agents, IEnumerable<AgentTask> tasks)
{
    
    public string Run()
    {
        string? context = null;
        
        foreach (var task in tasks)
        {
            task.Tools.Add(new AskQuestionTool(agents.Except(new []{task.Agent})));
            task.Tools.Add(new DelegateWorkTool(agents.Except(new[] { task.Agent })));
            var res = task.Execute(context);
            context = res;
        }

        return context;

    }



}
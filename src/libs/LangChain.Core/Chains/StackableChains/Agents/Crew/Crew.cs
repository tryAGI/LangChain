using LangChain.Chains.StackableChains.Agents.Crew.Tools;

namespace LangChain.Chains.StackableChains.Agents.Crew;

/// <summary>
/// 
/// </summary>
/// <param name="agents"></param>
/// <param name="tasks"></param>
public class Crew(
    IEnumerable<CrewAgent> agents,
    IEnumerable<AgentTask> tasks)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> RunAsync(CancellationToken cancellationToken = default)
    {
        string? context = null;
        
        foreach (var task in tasks)
        {
            task.Tools.Add(new AskQuestionTool(agents.Except(new []{task.Agent})));
            task.Tools.Add(new DelegateWorkTool(agents.Except(new[] { task.Agent })));
            var res = await task.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
            context = res;
        }

        return context ?? string.Empty;
    }
}
using LangChain.Chains.StackableChains.Agents.Crew.Tools;

namespace LangChain.Chains.StackableChains.Agents.Crew;

/// <summary>
/// 
/// </summary>
/// <param name="agent"></param>
/// <param name="description"></param>
/// <param name="tools"></param>
public class AgentTask(
    CrewAgent agent,
    string description,
    List<CrewAgentTool>? tools = null)
{
    /// <summary>
    /// 
    /// </summary>
    public CrewAgent Agent { get; set; } = agent;
    
    /// <summary>
    /// 
    /// </summary>
    public List<CrewAgentTool> Tools { get; set; } = tools ?? new List<CrewAgentTool>();
    
    /// <summary>
    /// 
    /// </summary>
    public string Description { get; set; } = description;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> ExecuteAsync(
        string? context = null,
        CancellationToken cancellationToken = default)
    {
        Agent.AddTools(Tools);
        Agent.Context = context;
        var chain = Chain.Set(Description, "task") 
                    | Agent;
        var res = await chain.RunAsync("result", cancellationToken: cancellationToken).ConfigureAwait(false) ?? string.Empty;
        return res;
    }
}
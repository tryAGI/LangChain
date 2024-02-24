namespace LangChain.Chains.StackableChains.Agents.Tools;

/// <summary>
/// 
/// </summary>
public abstract class AgentTool
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    protected AgentTool(
        string name,
        string? description = null)
    {
        Name = name;
        Description = description ?? string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract Task<string> ToolTask(string input, CancellationToken token = default);
}
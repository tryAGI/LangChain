namespace LangChain.Chains.StackableChains.Agents.Tools;

/// <summary>
/// 
/// </summary>
public class AgentToolLambda : AgentTool
{
    private readonly Func<string, Task<string>> _func;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="func"></param>
    public AgentToolLambda(string name, string description, Func<string, Task<string>> func) : base(name, description)
    {
        _func = func;
    }

    /// <inheritdoc />
    public override Task<string> ToolTask(string input, CancellationToken cancellationToken = default)
    {
        return _func(input);
    }
}
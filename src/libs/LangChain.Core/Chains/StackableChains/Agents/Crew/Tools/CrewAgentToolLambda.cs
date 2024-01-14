namespace LangChain.Chains.StackableChains.Agents.Crew.Tools;

public class CrewAgentToolLambda : CrewAgentTool
{
    private readonly Func<string, string> _func;

    public CrewAgentToolLambda(string name, string description, Func<string, string> func) : base(name, description)
    {
        _func = func;
    }

    public override string ToolAction(string input)
    {
        return _func(input);
    }
}
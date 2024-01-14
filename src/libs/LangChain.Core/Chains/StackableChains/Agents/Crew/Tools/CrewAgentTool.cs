namespace LangChain.Chains.StackableChains.Agents.Crew.Tools;

public abstract class CrewAgentTool
{
    public CrewAgentTool(string name, string description=null)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }


    public string Description { get; set; }


    public abstract string ToolAction(string input);
}
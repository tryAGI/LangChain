namespace LangChain.Chains.StackableChains.ReAct;

public class ReActAgentTool
{
    public ReActAgentTool(string name, string description, Func<string, string> func)
    {
        Name = name;
        Description = description;
        ToolCall = func;
    }

    public string Name { get; set; }
    public string Description { get; set; }

    public Func<string, string> ToolCall { get; set; }

}
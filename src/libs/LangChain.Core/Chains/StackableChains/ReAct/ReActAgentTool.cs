namespace LangChain.Chains.StackableChains.ReAct;

/// <summary>
/// 
/// </summary>
public class ReActAgentTool
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="func"></param>
    public ReActAgentTool(string name, string description, Func<string, string> func)
    {
        Name = name;
        Description = description;
        ToolCall = func;
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
    public Func<string, string> ToolCall { get; set; }
}
namespace LangChain.Prompts;

/// <summary>
/// 
/// </summary>
public class VariableNode(
    string name)
    : ParsedFStringNode("variable")
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; } = name;
}
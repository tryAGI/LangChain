namespace LangChain.Prompts;

/// <summary>
/// 
/// </summary>
/// <param name="type"></param>
public abstract class ParsedFStringNode(
    string type)
{
    /// <summary>
    /// 
    /// </summary>
    public string Type { get; } = type;
}
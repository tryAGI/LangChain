namespace LangChain.Schema;

/// <summary>
/// 
/// </summary>
public class PartialValues(
    Dictionary<string, string> value)
{
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string> Value { get; set; } = value;
}
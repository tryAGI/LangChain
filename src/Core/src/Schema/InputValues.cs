namespace LangChain.Schema;

/// <summary>
/// 
/// </summary>
/// <param name="value"></param>
public class InputValues(
    Dictionary<string, object> value)
{
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, object> Value { get; } = value;
}
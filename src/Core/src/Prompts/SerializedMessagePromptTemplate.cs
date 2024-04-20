namespace LangChain.Prompts;

/// <summary>
/// 
/// </summary>
public class SerializedMessagePromptTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public string Type { get; set; } = "message";
    
    /// <summary>
    /// 
    /// </summary>
    public List<string> InputVariables { get; set; } = new List<string>();

    /// <summary>
    /// 
    /// </summary>
    // Additional properties are handled by a dictionary
    public Dictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();
}
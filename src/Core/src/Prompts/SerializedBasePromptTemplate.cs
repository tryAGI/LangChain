namespace LangChain.Prompts;

/// <summary>
/// 
/// </summary>
public class SerializedBasePromptTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public List<string> InputVariables { get; set; } = new List<string>();

    //public TemplateFormat TemplateFormat { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Template { get; set; } = string.Empty;
}
namespace LangChain.Prompts;

public class SerializedBasePromptTemplate
{
    public string Type { get; set; } = string.Empty;

    public List<string> InputVariables { get; set; } = new List<string>();

    //public TemplateFormat TemplateFormat { get; set; }
    public string Template { get; set; } = string.Empty;
}
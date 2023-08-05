namespace LangChain.Prompts;

public class SerializedBasePromptTemplate
{
    public string Type { get; set; }
    
    public List<string> InputVariables { get; set; }
    
    //public TemplateFormat TemplateFormat { get; set; }
    public string Template { get; set; }
}
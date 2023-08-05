namespace LangChain.Prompts;

public class SerializedMessagePromptTemplate
{
    public string Type { get; set; } = "message";
    public List<string> InputVariables { get; set; }

    // Additional properties are handled by a dictionary
    public Dictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();
}
namespace LangChain.NET.Schema;

public class PartialValues
{
    public PartialValues(Dictionary<string, string> value)
    {
        this.Value = value;
    }
    
    public Dictionary<string, string> Value { get; set; } = new();
}
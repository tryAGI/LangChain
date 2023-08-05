namespace LangChain.NET.Schema;

public class InputValues
{
    public InputValues(Dictionary<string, object> value)
    {
        this.Value = value;
    }
    
    public Dictionary<string, object> Value { get; set; } = new();
}
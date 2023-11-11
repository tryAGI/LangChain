namespace LangChain.Schema;

public class OutputValues
{
    public OutputValues(Dictionary<string, object> value)
    {
        this.Value = value;
    }

    public Dictionary<string, object> Value { get; set; } = new();
}
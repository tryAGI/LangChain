using LangChain.Abstractions.Schema;

namespace LangChain.Schema;

public class ChainValues : IChainValues
{
    public ChainValues(object? getFinalOutput)
    {
        Value = new Dictionary<string, object>()
        {
            {"text", getFinalOutput}
        };
    }

    public ChainValues(string inputKey, object value)
    {
        Value = new Dictionary<string, object>()
        {
            {inputKey, value}
        };
    }

    public ChainValues(Dictionary<string, object> value)
    {
        Value = value;
    }

    public ChainValues(ChainValues value)
    {
        Value = value.Value;
    }

    public ChainValues()
    {
        Value = new Dictionary<string, object>();
    }

    public Dictionary<string, object> Value { get; set; }
}

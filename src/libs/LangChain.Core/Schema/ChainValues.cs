using LangChain.Abstractions.Schema;

namespace LangChain.Schema;

/// <inheritdoc/>
public class ChainValues : IChainValues
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="getFinalOutput"></param>
    public ChainValues(object? getFinalOutput)
    {
        Value = new Dictionary<string, object>
        {
            ["text"] = getFinalOutput!,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="value"></param>
    public ChainValues(string inputKey, object value)
    {
        Value = new Dictionary<string, object>()
        {
            {inputKey, value}
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public ChainValues(Dictionary<string, object> value)
    {
        Value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChainValues(ChainValues value)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));
        
        Value = value.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    public ChainValues()
    {
        Value = new Dictionary<string, object>();
    }

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, object> Value { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public object this[string key]
    {
        get => Value[key];
        set => Value[key] = value;
    }
}

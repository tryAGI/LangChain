using LangChain.Callback;

namespace LangChain.Base;

/// <inheritdoc />
public class ChainInputs : IChainInputs
{
    /// <inheritdoc />
    public ICallbacks? Callbacks { get; set; }
    
    /// <inheritdoc />
    public List<string> Tags { get; set; } = new();
    
    /// <inheritdoc />
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    /// <inheritdoc />
    public bool Verbose { get; set; }
}
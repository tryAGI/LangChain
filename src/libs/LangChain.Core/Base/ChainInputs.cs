using LangChain.Callback;

namespace LangChain.Base;

/// <inheritdoc />
public class ChainInputs : IChainInputs
{
    public ICallbacks? Callbacks { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public bool Verbose { get; set; }
}
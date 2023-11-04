using LangChain.Callback;

namespace LangChain.Base;

/// <inheritdoc />
public class ChainInputs : IChainInputs
{
    public ICallbacks? Callbacks { get; set; }
    public List<string> Tags { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    public bool Verbose { get; set; }
}
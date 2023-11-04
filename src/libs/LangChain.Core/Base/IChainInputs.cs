using LangChain.Callback;

namespace LangChain.Base;

/// <inheritdoc />
public interface IChainInputs : IBaseLangChainParams
{
    /// <summary>
    /// Optional list of callback handlers (or callback manager). Defaults to None.
    /// Callback handlers are called throughout the lifecycle of a call to a chain,
    /// starting with on_chain_start, ending with on_chain_end or on_chain_error.
    /// Each custom chain can optionally call additional callback methods, see Callback docs
    /// for full details.
    /// </summary>
    public ICallbacks? Callbacks { get; set; }
    
    /// <summary>
    /// Optional list of tags associated with the chain. Defaults to None.
    /// These tags will be associated with each call to this chain,
    /// and passed as arguments to the handlers defined in `callbacks`.
    /// You can use these to eg identify a specific instance of a chain with its use case.
    /// </summary>
    public List<string> Tags { get; set; }
    
    /// <summary>
    /// Optional metadata associated with the chain. Defaults to None.
    /// This metadata will be associated with each call to this chain,
    /// and passed as arguments to the handlers defined in `callbacks`.
    /// You can use these to eg identify a specific instance of a chain with its use case.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; }
}
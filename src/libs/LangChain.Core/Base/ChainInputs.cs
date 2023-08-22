using LangChain.Callback;

namespace LangChain.Base;

/// <inheritdoc />
public class ChainInputs : IChainInputs
{
    /// <inheritdoc />
    public CallbackManager? CallbackManager { get; set; }

    /// <inheritdoc />
    public bool? Verbose { get; set; }
}
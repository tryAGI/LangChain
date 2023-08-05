using LangChain.NET.Callback;

namespace LangChain.NET.Base;

public class ChainInputs : IChainInputs
{
    public CallbackManager? CallbackManager { get; set; }
    public bool? Verbose { get; set; }
}
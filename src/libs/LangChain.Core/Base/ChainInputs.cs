using LangChain.Callback;

namespace LangChain.Base;

public class ChainInputs : IChainInputs
{
    public CallbackManager? CallbackManager { get; set; }
    public bool? Verbose { get; set; }
}
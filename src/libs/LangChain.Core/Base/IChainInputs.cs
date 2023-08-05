using LangChain.Callback;

namespace LangChain.Base;

public interface IChainInputs : IBaseLangChainParams
{
    CallbackManager? CallbackManager { get; set; }
}
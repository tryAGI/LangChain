using LangChain.NET.Callback;

namespace LangChain.NET.Base;

public interface IChainInputs : IBaseLangChainParams
{
    CallbackManager? CallbackManager { get; set; }
}
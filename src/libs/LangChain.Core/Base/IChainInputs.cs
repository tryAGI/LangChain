using LangChain.Callback;

namespace LangChain.Base;

/// <inheritdoc />
public interface IChainInputs : IBaseLangChainParams
{
    /// <summary>
    /// 
    /// </summary>
    CallbackManager? CallbackManager { get; set; }
}
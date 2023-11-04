using LangChain.Abstractions.Schema;
using LangChain.Callback;

namespace LangChain.Abstractions.Chains.Base;

public interface IChain
{
    string[] InputKeys { get; }
    string[] OutputKeys { get; }

    Task<IChainValues> CallAsync(
        IChainValues values,
        Callbacks? callbacks = null,
        List<string>? tags = null,
        Dictionary<string, object>? metadata = null);
}
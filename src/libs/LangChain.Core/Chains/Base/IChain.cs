using LangChain.Abstractions.Schema;
using LangChain.Callback;

namespace LangChain.Abstractions.Chains.Base;

public interface IChain
{
    string[] InputKeys { get; }
    string[] OutputKeys { get; }
    
    Task<string?> Run(string input);
    Task<string> Run(Dictionary<string, object> input, ICallbacks? callbacks = null);
    
    Task<IChainValues> CallAsync(IChainValues values,
        ICallbacks? callbacks = null,
        IReadOnlyList<string>? tags = null,
        IReadOnlyDictionary<string, object>? metadata = null);
}
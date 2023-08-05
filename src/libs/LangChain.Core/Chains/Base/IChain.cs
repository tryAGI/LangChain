using LangChain.Abstractions.Schema;

namespace LangChain.Abstractions.Chains.Base;

public interface IChain
{
    string[] InputKeys { get; }
    string[] OutputKeys { get; }
    Task<IChainValues> CallAsync(IChainValues chainValues);
}
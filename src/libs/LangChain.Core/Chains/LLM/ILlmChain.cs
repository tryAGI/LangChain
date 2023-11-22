using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Schema;

namespace LangChain.Chains.LLM;

public interface ILlmChain : IChain, ILlmChainInput
{
    Task<object> Predict(ChainValues values);
    Task<List<IChainValues>> ApplyAsync(IReadOnlyList<ChainValues> inputs);
}
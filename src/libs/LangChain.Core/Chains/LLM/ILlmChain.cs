using LangChain.Abstractions.Chains.Base;
using LangChain.Schema;

namespace LangChain.Chains.LLM;

public interface ILlmChain : IChain, ILlmChainInput
{
    Task<object> Predict(ChainValues values);
}
using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Schema;

namespace LangChain.Chains.LLM;

/// <summary>
/// 
/// </summary>
public interface ILlmChain : IChain, ILlmChainInput
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    Task<object> Predict(ChainValues values);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    Task<List<IChainValues>> ApplyAsync(IReadOnlyList<ChainValues> inputs);
}
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<object> PredictAsync(ChainValues values, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<IChainValues>> ApplyAsync(IReadOnlyList<ChainValues> inputs, CancellationToken cancellationToken = default);
}
using LangChain.Schema;

namespace LangChain.Chains.StackableChains.Context;

/// <summary>
/// 
/// </summary>
public class StackableChainValues : ChainValues
{
    /// <summary>
    /// 
    /// </summary>
    public StackableChainHook? Hook { get; set; }
}
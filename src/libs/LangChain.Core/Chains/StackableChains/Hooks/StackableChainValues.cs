using LangChain.Schema;

namespace LangChain.Chains.StackableChains.Context;

public class StackableChainValues : ChainValues
{
    public StackableChainHook? Hook { get; set; }
}
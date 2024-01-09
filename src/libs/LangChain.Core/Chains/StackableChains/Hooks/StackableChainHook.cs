using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Context;

public class StackableChainHook
{
    public virtual void ChainStart(StackableChainValues values)
    {

    }

    public virtual void LinkEnter(BaseStackableChain chain, StackableChainValues values)
    {

    }

    public virtual void LinkExit(BaseStackableChain chain, StackableChainValues values)
    {

    }
}
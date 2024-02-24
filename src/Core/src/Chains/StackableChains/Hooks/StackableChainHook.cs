using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Context;

/// <summary>
/// 
/// </summary>
public class StackableChainHook
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    public virtual void ChainStart(StackableChainValues values)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chain"></param>
    /// <param name="values"></param>
    public virtual void LinkEnter(BaseStackableChain chain, StackableChainValues values)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chain"></param>
    /// <param name="values"></param>
    public virtual void LinkExit(BaseStackableChain chain, StackableChainValues values)
    {
    }
}
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains;

public class DoChain:BaseStackableChain
{
    private readonly Action<Dictionary<string, object>> _func;

    public DoChain(Action<Dictionary<string, object>> func)
    {
        _func = func;
    }
    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        _func(values.Value);
        return Task.FromResult(values);
    }
}
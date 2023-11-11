using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;

namespace LangChain.Chains.HelperChains;

public class SetChain : BaseStackableChain
{
    private readonly string _query;
    public SetChain(string query, string outputKey = "query")
    {
        OutputKeys = new[] { outputKey };
        _query = query;
    }

    protected override Task<IChainValues> InternallCall(IChainValues values)
    {
        values.Value[OutputKeys[0]] = _query;
        return Task.FromResult(values);
    }


}
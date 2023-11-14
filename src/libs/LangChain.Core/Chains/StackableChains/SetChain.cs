using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;

namespace LangChain.Chains.HelperChains;

public class SetChain : BaseStackableChain
{
    public SetChain(string query, string outputKey = "query")
    {
        OutputKeys = new[] { outputKey };
        Query = query;
    }

    public string Query { get; set; }


    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values.Value[OutputKeys[0]] = Query;
        return Task.FromResult(values);
    }

    

}
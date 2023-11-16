using LangChain.Abstractions.Schema;

namespace LangChain.Chains.HelperChains;

public class SetLambdaChain: BaseStackableChain
{
    public SetLambdaChain(Func<string> queryGetter, string outputKey = "query")
    {
        OutputKeys = new[] { outputKey };
        QueryGetter = queryGetter;
    }

    public Func<string> QueryGetter { get; set; }


    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values.Value[OutputKeys[0]] = QueryGetter();
        return Task.FromResult(values);
    }
}
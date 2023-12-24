using LangChain.Abstractions.Schema;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc />
public class SetLambdaChain : BaseStackableChain
{
    /// <inheritdoc />
    public SetLambdaChain(Func<string> queryGetter, string outputKey = "query")
    {
        OutputKeys = new[] { outputKey };
        QueryGetter = queryGetter;
    }

    /// <summary>
    /// 
    /// </summary>
    public Func<string> QueryGetter { get; set; }

    /// <inheritdoc />
    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        values.Value[OutputKeys[0]] = QueryGetter();
        return Task.FromResult(values);
    }
}
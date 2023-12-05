using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;

namespace LangChain.Chains.HelperChains;

public class SetChain : BaseStackableChain
{
    public SetChain(object value, string outputKey = "query")
    {
        OutputKeys = new[] { outputKey };
        Value = value;
    }

    public object Value { get; set; }


    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        values.Value[OutputKeys[0]] = Value;
        return Task.FromResult(values);
    }
}
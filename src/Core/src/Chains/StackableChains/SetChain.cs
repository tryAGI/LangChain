using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class SetChain : BaseStackableChain
{
    /// <inheritdoc/>
    public SetChain(object value, string outputKey = "query")
    {
        OutputKeys = new[] { outputKey };
        Value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public object Value { get; set; }

    /// <inheritdoc/>
    protected override Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        values.Value[OutputKeys[0]] = Value;
        return Task.FromResult(values);
    }
}
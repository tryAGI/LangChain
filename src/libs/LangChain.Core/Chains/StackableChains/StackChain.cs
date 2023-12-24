using LangChain.Abstractions.Schema;
using LangChain.Schema;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class StackChain(
    BaseStackableChain a,
    BaseStackableChain b)
    : BaseStackableChain
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<string> IsolatedInputKeys { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<string> IsolatedOutputKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputKeys"></param>
    /// <param name="outputKeys"></param>
    /// <returns></returns>
    public StackChain AsIsolated(
        string[]? inputKeys = null,
        string[]? outputKeys = null)
    {
        IsolatedInputKeys = inputKeys ?? IsolatedInputKeys;
        IsolatedOutputKeys = outputKeys ?? IsolatedOutputKeys;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    /// <returns></returns>
    public StackChain AsIsolated(
        string? inputKey = null,
        string? outputKey = null)
    {
        if (inputKey != null) IsolatedInputKeys = new[] { inputKey };
        if (outputKey != null) IsolatedOutputKeys = new[] { outputKey };
        
        return this;
    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        // since it is reference type, the values would be changed anyhow
        var originalValues = values;

        if (IsolatedInputKeys.Count > 0)
        {
            var res = new ChainValues();
            foreach (var key in IsolatedInputKeys)
            {
                res.Value[key] = values.Value[key];
            }
            values = res;
        }
        await a.CallAsync(values).ConfigureAwait(false);
        await b.CallAsync(values).ConfigureAwait(false);
        if (IsolatedOutputKeys.Count > 0)
        {
            foreach (var key in IsolatedOutputKeys)
            {
                originalValues.Value[key] = values.Value[key];
            }

        }
        return originalValues;
    }



    
}
using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Schema;

namespace LangChain.Chains.HelperChains;

public class StackChain : BaseStackableChain
{
    private readonly BaseStackableChain _a;
    private readonly BaseStackableChain _b;

    public string[] IsolatedInputKeys { get; set; } = Array.Empty<string>();
    public string[] IsolatedOutputKeys { get; set; } = Array.Empty<string>();

    public StackChain(BaseStackableChain a, BaseStackableChain b)
    {
        _a = a;
        _b = b;
    }

    public StackChain AsIsolated(
        string[]? inputKeys = null,
        string[]? outputKeys = null)
    {
        IsolatedInputKeys = inputKeys ?? IsolatedInputKeys;
        IsolatedOutputKeys = outputKeys ?? IsolatedOutputKeys;
        return this;
    }

    public StackChain AsIsolated(
        string? inputKey = null,
        string? outputKey = null)
    {
        if (inputKey != null) IsolatedInputKeys = new[] { inputKey };
        if (outputKey != null) IsolatedOutputKeys = new[] { outputKey };
        
        return this;
    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        // since it is reference type, the values would be changed anyhow
        var originalValues = values;

        if (IsolatedInputKeys.Length > 0)
        {
            var res = new ChainValues();
            foreach (var key in IsolatedInputKeys)
            {
                res.Value[key] = values.Value[key];
            }
            values = res;
        }
        await _a.CallAsync(values).ConfigureAwait(false);
        await _b.CallAsync(values).ConfigureAwait(false);
        if (IsolatedOutputKeys.Length > 0)
        {
            foreach (var key in IsolatedOutputKeys)
            {
                originalValues.Value[key] = values.Value[key];
            }

        }
        return originalValues;
    }



    
}
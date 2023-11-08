using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Schema;

namespace LangChain.Chains.HelperChains;

public class StackChain:BaseStackableChain
{
    private readonly BaseStackableChain _a;
    private readonly BaseStackableChain _b;

    public string[] IsolatedInputKeys { get; set; }=new string[0];
    public string[] IsolatedOutputKeys { get; set; }=new string[0];

    public StackChain(BaseStackableChain a, BaseStackableChain b)
    {
        _a = a;
        _b = b;

    }

    public StackChain AsIsolated(string[] inputKeys = null, string[] outputKeys = null)
    {
        IsolatedInputKeys = inputKeys ?? IsolatedInputKeys;
        IsolatedOutputKeys = outputKeys ?? IsolatedOutputKeys;
        return this;
    }

    public StackChain AsIsolated(string inputKey = null, string outputKey = null)
    {
        if (inputKey != null) IsolatedInputKeys = new[] { inputKey };
        if (outputKey != null) IsolatedOutputKeys = new[] { outputKey };
        return this;
    }

    protected override async Task<IChainValues> InternallCall(IChainValues values)
    {
        // since it is reference type, the values would be changed anyhow
        var originalValues = values;

        if (IsolatedInputKeys.Length>0)
        {
            var res = new ChainValues();
            foreach (var key in IsolatedInputKeys)
            {
                res.Value[key] = values.Value[key];
            }
            values = res;
        }
        await _a.CallAsync(values);
        await _b.CallAsync(values);
        if (IsolatedOutputKeys.Length > 0)
        {
     
            foreach (var key in IsolatedOutputKeys)
            {
                originalValues.Value[key] = values.Value[key];
            }
    
        }
        return originalValues;
    }



    public async Task<IChainValues> Run()
    {
        
        var res = await CallAsync(new ChainValues());
        return res;
    }

    public async Task<string> Run(string resultKey)
    {
        var res = await CallAsync(new ChainValues());
        return res.Value[resultKey].ToString();
    }
}
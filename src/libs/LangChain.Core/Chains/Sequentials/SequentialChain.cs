using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Schema;

namespace LangChain.Chains.Sequentials;

/// <inheritdoc />
public class SequentialChain : BaseChain
{
    /// <summary>
    /// 
    /// </summary>
    public IChain[] Chains { get; }
    
    /// <inheritdoc />
    public override string[] InputKeys { get; }
    
    /// <inheritdoc />
    public override string[] OutputKeys { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ReturnAll { get; }

    private HashSet<string> _allOutputKeys;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public SequentialChain(SequentialChainInput input)
    {
        Chains = input.Chains;
        InputKeys = input.InputVariables;
        OutputKeys = input.OutputVariables ?? Array.Empty<string>();
        ReturnAll = input.ReturnAll;
        
        Validate();
        
        if(OutputKeys.Length == 0 && !ReturnAll)
        {
            OutputKeys = Chains[^1].OutputKeys;
        }
        

    }
    
    public override string ChainType()
    {
        return "sequential_chain";
    }
    public override async Task<IChainValues> CallAsync(IChainValues values)
    {
        var allChainValues = new ChainValues(new  Dictionary<string, object>(_allOutputKeys.Count));
        foreach (var input in InputKeys)  
        {
            allChainValues.Value[input] = values.Value[input];
        }

        foreach (var chain in Chains)
        {
            var input = await chain.CallAsync(allChainValues);

            foreach (var inputValue in input.Value)
            {
                allChainValues.Value[inputValue.Key] = inputValue.Value;
            }
        }

        if (ReturnAll)
        {
            foreach (var key in Chains[0].InputKeys)
            {
                allChainValues.Value.Remove(key);
            }
            return new ChainValues(allChainValues.Value.ToDictionary(_ => _.Key, _ => _.Value));
        }

        var output = new ChainValues();
        foreach (var outputKey in OutputKeys)
        {
            output.Value.Add(outputKey, allChainValues.Value[outputKey]);
        }

        return output;
    }

    protected virtual void Validate()
    {
        if (OutputKeys.Length > 0 && ReturnAll)
        {
            throw new ArgumentException(
                "Either specify variables to return using `outputVariables` or use `returnAll` param. Cannot apply both conditions at the same time.");
        }

        if (Chains.Length == 0)
        {
            throw new ArgumentException("Sequential chain must have at least one chain.");
        }

#if NET6_0_OR_GREATER
        var allOutputKeysCount = Chains.Sum(_ => _.OutputKeys.Length) + InputKeys.Length;
        _allOutputKeys = new HashSet<string>(allOutputKeysCount);
#else
        _allOutputKeys = new HashSet<string>();
#endif
        
        _allOutputKeys.UnionWith(InputKeys);
        foreach (var chain in Chains)
        {
            foreach (var chainOutputKey in chain.OutputKeys)
            {
                if (_allOutputKeys.Contains(chainOutputKey))
                {
                    throw new ArgumentException($"Duplicate output key `{chainOutputKey}`");
                }
            }
            _allOutputKeys.UnionWith(chain.OutputKeys);
        }
    }
}
using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Base;
using LangChain.Callback;
using LangChain.Schema;

namespace LangChain.Chains.Sequentials;

/// <inheritdoc />
public class SequentialChain : BaseChain
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<IChain> Chains { get; }

    /// <inheritdoc />
    public override IReadOnlyList<string> InputKeys { get; }

    /// <inheritdoc />
    public override IReadOnlyList<string> OutputKeys { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool ReturnAll { get; }

    private HashSet<string> _allOutputKeys = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    public SequentialChain(SequentialChainInput input) : base(input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        Chains = input.Chains;
        InputKeys = input.InputVariables;
        OutputKeys = input.OutputVariables ?? Array.Empty<string>();
        ReturnAll = input.ReturnAll;

        Validate();

        if (OutputKeys.Count == 0 && !ReturnAll)
        {
            OutputKeys = Chains[^1].OutputKeys;
        }
    }

    /// <inheritdoc/>
    public override string ChainType() => "sequential_chain";

    /// <inheritdoc/>
    protected override async Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager, CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var allChainValues = new ChainValues(new Dictionary<string, object>(_allOutputKeys.Count));
        foreach (var input in InputKeys)
        {
            allChainValues.Value[input] = values.Value[input];
        }

        foreach (var chain in Chains)
        {
            var input = await chain.CallAsync(allChainValues, cancellationToken: cancellationToken).ConfigureAwait(false);

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

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    protected virtual void Validate()
    {
        if (OutputKeys.Count > 0 && ReturnAll)
        {
            throw new ArgumentException(
                "Either specify variables to return using `outputVariables` or use `returnAll` param. Cannot apply both conditions at the same time.");
        }

        if (Chains.Count == 0)
        {
            throw new ArgumentException("Sequential chain must have at least one chain.");
        }

#if NET6_0_OR_GREATER
        var allOutputKeysCount = Chains.Sum(_ => _.OutputKeys.Count) + InputKeys.Count;
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
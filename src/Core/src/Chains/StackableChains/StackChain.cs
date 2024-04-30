using LangChain.Abstractions.Schema;
using LangChain.Chains.StackableChains.Context;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class StackChain(
    BaseStackableChain a,
    BaseStackableChain b)
    : BaseStackableChain(b)
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
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var stackableChainValues = values as StackableChainValues;
        var hook = stackableChainValues?.Hook;
        // since it is reference type, the values would be changed anyhow
        var originalValues = values;

        if (IsolatedInputKeys.Count > 0)
        {
            var res = new StackableChainValues
            {
                Hook = hook,
            };
            foreach (var key in IsolatedInputKeys)
            {
                res.Value[key] = values.Value[key];
            }
            values = res;
        }

        if (a is not StackChain &&
            stackableChainValues != null)
        {
            hook?.LinkEnter(a, stackableChainValues);
        }

        await a.CallAsync(values, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (a is not StackChain &&
            stackableChainValues != null)
        {
            hook?.LinkExit(a, stackableChainValues);
        }
        if (b is not StackChain &&
            stackableChainValues != null)
        {
            hook?.LinkEnter(b, stackableChainValues);
        }

        await b.CallAsync(values, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (b is not StackChain &&
            stackableChainValues != null)
        {
            hook?.LinkExit(b, stackableChainValues);
        }

        if (IsolatedOutputKeys.Count > 0)
        {
            foreach (var key in IsolatedOutputKeys)
            {
                originalValues.Value[key] = values.Value[key];
            }
        }

        return originalValues;
    }

    /// <summary>
    /// Represents a stack chain.
    /// </summary>
    public static StackChain operator >>(StackChain a, BaseStackableChain b)
    {
        a = a ?? throw new ArgumentNullException(nameof(a));

        return a.AsIsolated(outputKey: a.OutputKeys[^1]) | b;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static StackChain RightShift(StackChain left, BaseStackableChain right)
    {
        return left >> right;
    }
}
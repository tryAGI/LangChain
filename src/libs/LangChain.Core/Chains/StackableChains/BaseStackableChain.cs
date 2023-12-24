using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Chains.HelperChains.Exceptions;
using LangChain.Schema;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public abstract class BaseStackableChain : IChain
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <inheritdoc/>
    public virtual IReadOnlyList<string> InputKeys { get; protected set; } = Array.Empty<string>();
    /// <inheritdoc/>
    public virtual IReadOnlyList<string> OutputKeys { get; protected set; } = Array.Empty<string>();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GenerateName()
    {
        return GetType().Name;
    }

    private string GetInputs()
    {
        return string.Join(",", InputKeys);
    }

    private string GetOutputs()
    {
        return string.Join(",", OutputKeys);
    }

    string FormatInputValues(IChainValues values)
    {
        List<string> res = new();
        foreach (var key in InputKeys)
        {
            if (!values.Value.ContainsKey(key))
            {
                res.Add($"{key} is expected but missing");
                continue;
            };
            res.Add($"{key}={values.Value[key]}");
        }
        return string.Join(",\n", res);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="callbacks"></param>
    /// <param name="tags"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="StackableChainException"></exception>
    public Task<IChainValues> CallAsync(IChainValues values, ICallbacks? callbacks = null,
        IReadOnlyList<string>? tags = null, IReadOnlyDictionary<string, object>? metadata = null)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }
        try
        {
            return InternalCall(values);
        }
        catch (StackableChainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var name = string.IsNullOrWhiteSpace(Name)
                ? GenerateName()
                : Name;
            var inputValues = FormatInputValues(values);
            var message = $"Error occured in {name} with inputs \n{inputValues}\n.";

            throw new StackableChainException(message, ex);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    protected abstract Task<IChainValues> InternalCall(IChainValues values);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static StackChain operator |(BaseStackableChain a, BaseStackableChain b)
    {
        return new StackChain(a, b);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static StackChain BitwiseOr(BaseStackableChain left, BaseStackableChain right)
    {
        return left | right;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IChainValues> Run()
    {
        var res = await CallAsync(new ChainValues()).ConfigureAwait(false);
        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resultKey"></param>
    /// <returns></returns>
    public async Task<string?> Run(string resultKey)
    {
        var res = await CallAsync(new ChainValues()).ConfigureAwait(false);
        return res.Value[resultKey].ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="callbacks"></param>
    /// <returns></returns>
    public async Task<string> Run(
        Dictionary<string, object> input,
        ICallbacks? callbacks = null)
    {
        var res = await CallAsync(new ChainValues(input)).ConfigureAwait(false);

        return res.Value[OutputKeys[0]].ToString() ?? string.Empty;
    }
}
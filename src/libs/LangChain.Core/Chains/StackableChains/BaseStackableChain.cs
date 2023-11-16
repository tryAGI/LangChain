using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Chains.HelperChains.Exceptions;
using LangChain.Schema;

namespace LangChain.Chains.HelperChains;

public abstract class BaseStackableChain : IChain
{
    public string Name { get; set; }
    public virtual string[] InputKeys { get; protected set; }
    public virtual string[] OutputKeys { get; protected set; }

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

    public Task<IChainValues> CallAsync(IChainValues values, ICallbacks? callbacks = null,
        List<string>? tags = null, Dictionary<string, object>? metadata = null)
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
            var name = Name ?? GenerateName();
            var inputValues = FormatInputValues(values);
            var message = $"Error occured in {name} with inputs \n{inputValues}\n.";

            throw new StackableChainException(message, ex);
        }

    }

    protected abstract Task<IChainValues> InternalCall(IChainValues values);

    public static StackChain operator |(BaseStackableChain a, BaseStackableChain b)
    {
        return new StackChain(a, b);
    }

    public static StackChain BitwiseOr(BaseStackableChain left, BaseStackableChain right)
    {
        return left | right;
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
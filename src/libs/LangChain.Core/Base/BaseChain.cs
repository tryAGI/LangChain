using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Chains;
using LangChain.Schema;

namespace LangChain.Base;

using System.Collections.Generic;
using LoadValues = Dictionary<string, object>;

/// <inheritdoc />
public abstract class BaseChain(IChainInputs fields) : IChain
{
    const string RunKey = "__run";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract string ChainType();

    /// <summary>
    /// 
    /// </summary>
    public abstract string[] InputKeys { get; }

    /// <summary>
    /// 
    /// </summary>
    public abstract string[] OutputKeys { get; }

    /// <summary>
    /// Run the chain using a simple input/output.
    /// </summary>
    /// <param name="input">The string input to use to execute the chain.</param>
    /// <returns>A text value containing the result of the chain.</returns>
    /// <exception cref="ArgumentException">If the type of chain used expects multiple inputs, this method will throw an ArgumentException.</exception>
    public virtual async Task<string?> Run(string input)
    {
        var isKeylessInput = InputKeys.Length <= 1;

        if (!isKeylessInput)
        {
            throw new ArgumentException($"Chain {ChainType()} expects multiple inputs, cannot use 'run'");
        }

        var values = InputKeys.Length > 0 ? new ChainValues(InputKeys[0], input) : new ChainValues();
        var returnValues = await CallAsync(values);
        var keys = returnValues.Value.Keys;

        if (keys.Count(p => p != RunKey) == 1)
        {
            var returnValue = returnValues.Value.FirstOrDefault(p => p.Key != RunKey).Value;

            return returnValue?.ToString();
        }

        throw new Exception("Return values have multiple keys, 'run' only supported when one key currently");
    }

    /// <summary>
    /// Run the chain using a simple input/output.
    /// </summary>
    /// <param name="input">The dict input to use to execute the chain.</param>
    /// <returns>A text value containing the result of the chain.</returns>
    public virtual async Task<string> Run(Dictionary<string, object> input)
    {
        var keysLengthDifferent = InputKeys.Length != input.Count;

        if (keysLengthDifferent)
        {
            throw new ArgumentException($"Chain {ChainType()} expects {InputKeys.Length} but, received {input.Count}");
        }

        var returnValues = await CallAsync(new ChainValues(input));

        var returnValue = returnValues.Value.FirstOrDefault(kv => kv.Key == OutputKeys[0]).Value;

        return returnValue?.ToString();
    }

    /// <summary>
    /// Execute the chain, using the values provided.
    /// </summary>
    /// <param name="values">The <see cref="ChainValues"/> to use.</param>
    /// <param name="callbacks"></param>
    /// <param name="tags"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public async Task<IChainValues> CallAsync(IChainValues values,
        ICallbacks? callbacks = null,
        IReadOnlyList<string>? tags = null,
        IReadOnlyDictionary<string, object>? metadata = null)
    {
        var callbackManager = await CallbackManager.Configure(
            callbacks,
            fields.Callbacks,
            fields.Verbose,
            tags,
            fields.Tags,
            metadata,
            fields.Metadata);

        var runManager = await callbackManager.HandleChainStart(this, values);

        try
        {
            var result = await CallAsync(values, runManager);

            await runManager.HandleChainEndAsync(values, result);

            return result;
        }
        catch (Exception e)
        {
            await runManager.HandleChainErrorAsync(e, values);
            throw;
        }
    }

    /// <summary>
    /// Execute the chain, using the values provided.
    /// </summary>
    /// <param name="values">The <see cref="ChainValues"/> to use.</param>
    /// <param name="runManager"></param>
    /// <returns></returns>
    protected abstract Task<IChainValues> CallAsync(IChainValues values, CallbackManagerForChainRun? runManager);

    /// <summary>
    /// Call the chain on all inputs in the list.
    /// </summary>
    public virtual async Task<List<IChainValues>> ApplyAsync(IReadOnlyList<ChainValues> inputs)
    {
        var tasks = inputs.Select(input=> CallAsync(input));
        var results = await Task.WhenAll(tasks);

        return results.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<BaseChain> Deserialize(SerializedBaseChain data, LoadValues? values = null)
    {
        switch (data.Type)
        {
            case "llm_chain":
                {
                    var llmChainType = Type.GetType("Namespace.LLMChain"); // Replace with the actual namespace and class name
                    var deserializeMethod = llmChainType?.GetMethod("Deserialize");

                    return await (Task<BaseChain>)deserializeMethod.Invoke(null, new object[] { data });
                }
            case "sequential_chain":
                {
                    var sequentialChainType = Type.GetType("Namespace.SequentialChain"); // Replace with the actual namespace and class name
                    var deserializeMethod = sequentialChainType.GetMethod("Deserialize");

                    return await (Task<BaseChain>)deserializeMethod.Invoke(null, new object[] { data });
                }
            case "simple_sequential_chain":
                {
                    var simpleSequentialChainType = Type.GetType("Namespace.SimpleSequentialChain"); // Replace with the actual namespace and class name
                    var deserializeMethod = simpleSequentialChainType.GetMethod("Deserialize");

                    return await (Task<BaseChain>)deserializeMethod.Invoke(null, new object[] { data });
                }
            default:
                throw new Exception($"Invalid prompt type in config: {data.Type}");
        }
    }
}

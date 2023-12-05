using LangChain.Schema;

namespace LangChain.Prompts.Base;

/// <summary>
/// 
/// </summary>
public abstract class BasePromptTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<string> InputVariables { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public InputValues PartialVariables { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <exception cref="Exception"></exception>
    public BasePromptTemplate(IBasePromptTemplateInput input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        
        if (input.InputVariables.Contains("stop"))
        {
            throw new ArgumentException("Cannot have an input variable named 'stop', as it is used internally, please rename.");
        }

        InputVariables = input.InputVariables;
        PartialVariables = new InputValues(input.PartialVariables);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public abstract Task<BasePromptTemplate> AddPartial(PartialValues values);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userVariables"></param>
    /// <returns></returns>
    public async Task<InputValues> MergePartialAndUserVariables(InputValues userVariables)
    {
        userVariables = userVariables ?? throw new ArgumentNullException(nameof(userVariables));
        
        InputValues partialValues = new InputValues(new Dictionary<string, object>());

        foreach (KeyValuePair<string, object> entry in PartialVariables.Value)
        {
            string key = entry.Key;
            object value = entry.Value;

            if (value is string stringValue)
            {
                partialValues.Value[key] = stringValue;
            }
            else if (value is Func<Task<string>> asyncFunc)
            {
                partialValues.Value[key] = await asyncFunc().ConfigureAwait(false);
            }
        }

        InputValues allKwargs = new InputValues(partialValues.Value.Concat(userVariables.Value).ToDictionary(x => x.Key, x => x.Value));
        return allKwargs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public abstract Task<string> Format(InputValues values);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public abstract Task<BasePromptValue> FormatPromptValue(InputValues values);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected abstract string GetPromptType();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract SerializedBasePromptTemplate Serialize();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async static Task<BasePromptTemplate> Deserialize(SerializedBasePromptTemplate data)
    {
        data = data ?? throw new ArgumentNullException(nameof(data));
        
        switch (data.Type)
        {
            case "prompt":
                {
                    var promptTemplate = await Deserialize(data).ConfigureAwait(false);
                    return promptTemplate;
                }
            case null:
                {
                    var promptTemplate = await Deserialize(new SerializedBasePromptTemplate { Type = "prompt" }).ConfigureAwait(false);
                    return promptTemplate;
                }
            default:
                throw new InvalidOperationException($"Invalid prompt type in config: {data.Type}");
        }
    }
}
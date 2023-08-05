using LangChain.NET.Schema;

namespace LangChain.NET.Prompts.Base;

public abstract class BasePromptTemplate
{
    public List<string> InputVariables { get; private set;  }
    public InputValues PartialVariables { get; }
    
    public BasePromptTemplate(IBasePromptTemplateInput input)
    {
        if (input.InputVariables.Contains("stop"))
        {
            throw new Exception("Cannot have an input variable named 'stop', as it is used internally, please rename.");
        }
        
        InputVariables = input.InputVariables;
        PartialVariables = new InputValues(input.PartialVariables);
    }
    
    public abstract Task<BasePromptTemplate> Partial(PartialValues values);
    
    public async Task<InputValues> MergePartialAndUserVariables(InputValues userVariables)
    {
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
                partialValues.Value[key] = await asyncFunc();
            }
        }
        
        InputValues allKwargs = new InputValues(partialValues.Value.Concat(userVariables.Value).ToDictionary(x => x.Key, x => x.Value));
        return allKwargs;
    }
    
    public abstract Task<string> Format(InputValues values);
    
    public abstract Task<BasePromptValue> FormatPromptValue(InputValues values);
    
    protected abstract string GetPromptType();
    
    public abstract SerializedBasePromptTemplate Serialize();
    
    public static async Task<BasePromptTemplate> Deserialize(SerializedBasePromptTemplate data)
    {
        switch (data.Type)
        {
            case "prompt":
                {
                    var promptTemplate = await Deserialize(data);
                    return promptTemplate;
                }
            case null:
                {
                    var promptTemplate = await Deserialize(new SerializedBasePromptTemplate { Type = "prompt" });
                    return promptTemplate;
                }
            default:
                throw new Exception($"Invalid prompt type in config: {data.Type}");
        }
    }
}
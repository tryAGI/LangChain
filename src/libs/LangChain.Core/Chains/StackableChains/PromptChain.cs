using System.Text.RegularExpressions;
using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Chains.LLM;
using LangChain.Prompts;

namespace LangChain.Chains.HelperChains;

public class PromptChain : BaseStackableChain
{
    private readonly string _template;

    public PromptChain(string template, string outputKey = "prompt")
    {
        OutputKeys = new[] { outputKey };
        _template = template;
        InputKeys = GetVariables().ToArray();
    }

    List<string> GetVariables()
    {
        string pattern = @"\{([^\{\}]+)\}";
        var variables = new List<string>();
        var matches = Regex.Matches(_template, pattern);
        foreach (Match match in matches)
        {
            variables.Add(match.Groups[1].Value);
        }
        return variables;
    }




    protected override Task<IChainValues> InternallCall(IChainValues values)
    {
        // validate that input keys containing all variables
        var valueKeys = values.Value.Keys;
        var missing = InputKeys.Except(valueKeys);
        if (missing.Any())
        {
            throw new Exception($"Input keys must contain all variables in template. Missing: {string.Join(",", missing)}");
        }

        var formattedPrompt = PromptTemplate.InterpolateFString(_template, values.Value);

        values.Value[OutputKeys[0]] = formattedPrompt;

        return Task.FromResult(values);
    }
}
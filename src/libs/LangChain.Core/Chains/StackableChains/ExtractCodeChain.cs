using System.Text;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains;

public class ExtractCodeChain: BaseStackableChain
{
    public ExtractCodeChain(string inputKey="text", string outputKey="code")
    {
        InputKeys = new[] {inputKey};
        OutputKeys = new[] {outputKey};
    }

    string ExtractCode(string md)
    {
        var lines = md.Split('\n');
        var code = new StringBuilder();
        var inCode = false;
        foreach (var line in lines)
        {
            if (line.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                inCode = !inCode;
                continue;
            }

            if (inCode)
            {
                code.AppendLine(line);
            }
        }

        if (code.Length == 0)
        {
            code.AppendLine(md);
        }

        return code.ToString();

    }


    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        if (values.Value[InputKeys[0]] is string text)
        {
            values.Value[OutputKeys[0]] = ExtractCode(text);
        }
        else
        {
            throw new InvalidOperationException($"Input key {InputKeys[0]} must be string");
        }
        
        return Task.FromResult(values);
    }
}
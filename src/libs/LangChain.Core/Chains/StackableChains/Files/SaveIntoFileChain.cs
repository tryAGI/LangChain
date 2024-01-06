using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Files;

public class SaveIntoFileChain:BaseStackableChain
{
    private readonly string _filename;

    public SaveIntoFileChain(string filename, string inputKey="data")
    {
        _filename = filename;
        InputKeys = new[] { inputKey };
        OutputKeys = Array.Empty<string>();
    }

    protected override Task<IChainValues> InternalCall(IChainValues values)
    {
        if (values.Value[InputKeys[0]] is byte[] data)
        {
            File.WriteAllBytes(_filename, data);
        }
        else if (values.Value[InputKeys[0]] is string text)
        {
            File.WriteAllText(_filename, text);
        }
        else
        {
            throw new InvalidOperationException($"Input key {InputKeys[0]} must be byte[] or string");
        }

        return Task.FromResult(values);
        
    }
}
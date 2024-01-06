using LangChain.Chains.HelperChains;

namespace LangChain.Chains.StackableChains.Files;

/// <summary>
/// 
/// </summary>
public class SaveIntoFileChain : BaseStackableChain
{
    private readonly string _filename;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="inputKey"></param>
    public SaveIntoFileChain(string filename, string inputKey="data")
    {
        _filename = filename;
        InputKeys = new[] { inputKey };
        OutputKeys = Array.Empty<string>();
    }

    /// <inheritdoc />
    protected override
#if NET6_0_OR_GREATER
        async
#endif
        Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        if (values.Value[InputKeys[0]] is byte[] data)
        {
#if NET6_0_OR_GREATER
            await File.WriteAllBytesAsync(_filename, data).ConfigureAwait(false);
#else
            File.WriteAllBytes(_filename, data);
#endif
        }
        else if (values.Value[InputKeys[0]] is string text)
        {
#if NET6_0_OR_GREATER
            await File.WriteAllTextAsync(_filename, text).ConfigureAwait(false);
#else
            File.WriteAllText(_filename, text);
#endif
        }
        else
        {
            throw new InvalidOperationException($"Input key {InputKeys[0]} must be byte[] or string");
        }
        
#if NET6_0_OR_GREATER
        return values;
#else
        return Task.FromResult(values);
#endif
    }
}

using LangChain.Abstractions.Schema;
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
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        if (values.Value[InputKeys[0]] is byte[] data)
        {
            await File2.WriteAllBytesAsync(_filename, data, cancellationToken).ConfigureAwait(false);
        }
        else if (values.Value[InputKeys[0]] is string text)
        {
            await File2.WriteAllTextAsync(_filename, text, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new InvalidOperationException($"Input key {InputKeys[0]} must be byte[] or string");
        }
        
        return values;
    }
}

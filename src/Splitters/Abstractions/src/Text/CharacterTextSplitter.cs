namespace LangChain.Splitters.Text;

/// <summary>
/// Implementation of splitting text that looks at characters
/// </summary>
public class CharacterTextSplitter(
    string? separator = "\n\n",
    int chunkSize = 4000,
    int chunkOverlap = 200,
    Func<string, int>? lengthFunction = null)
    : TextSplitter(chunkSize, chunkOverlap, lengthFunction)
{
    /// <inheritdoc/>
    public override IReadOnlyList<string> SplitText(string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));
        
        List<string> splits;
        if (separator != null)
        {
            splits = text.Split(new[] { separator }, StringSplitOptions.None).ToList();
        }
        else
        {
            splits = new List<string> { text };
        }
        
        return this.MergeSplits(splits, separator ?? string.Empty);
    }
}
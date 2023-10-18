using LangChain.Base;

namespace LangChain.TextSplitters;

/// <summary>
/// Implementation of splitting text that looks at characters
/// </summary>
public class CharacterTextSplitter:TextSplitter
{
    private readonly string? _separator;

    public CharacterTextSplitter(string? separator = "\n\n", int chunkSize = 4000, int chunkOverlap = 200, Func<string, int>? lengthFunction = null) : base(chunkSize, chunkOverlap, lengthFunction)
    {
        _separator = separator;
    }

    public override List<string> SplitText(string text)
    {
        List<string> splits;
        if (_separator!=null)
        {
            splits = text.Split(new[] { _separator }, StringSplitOptions.None).ToList();
        }
        else
        {
            splits = new List<string> { text};
        }
        return this.MergeSplits(splits,_separator);
    }
}
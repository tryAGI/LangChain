namespace LangChain.Splitters.Text;

/// <summary>
/// Implementation of splitting text that looks at characters.
/// Recursively tries to split by different characters to find one
/// that works.
/// </summary>
public class RecursiveCharacterTextSplitter(
    IReadOnlyList<string>? separators = null,
    int chunkSize = 4000,
    int chunkOverlap = 200,
    Func<string, int>? lengthFunction = null)
    : TextSplitter(chunkSize, chunkOverlap, lengthFunction)
{
    private readonly IReadOnlyList<string> _separators = separators ?? new List<string> { "\n\n", "\n", " ", "" };

    /// <inheritdoc/>
    public override IReadOnlyList<string> SplitText(string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        List<string> finalChunks = new List<string>();
        string separator = _separators[_separators.Count - 1];

        foreach (string _s in _separators)
        {
            if (_s.Length == 0)
            {
                separator = _s;
                break;
            }

            if (text.Contains(_s))
            {
                separator = _s;
                break;
            }
        }

        List<string> splits;
        if (separator.Length != 0)
        {
            splits = text.Split(new[] { separator }, StringSplitOptions.None).ToList();
        }
        else
        {
            splits = text.ToCharArray().Select(c => c.ToString()).ToList();
        }


        List<string> goodSplits = new List<string>();

        foreach (var s in splits)
        {
            if (s.Length < ChunkSize)
            {
                goodSplits.Add(s);
            }
            else
            {
                if (goodSplits.Count != 0)
                {
                    var mergedText = MergeSplits(goodSplits, separator);
                    finalChunks.AddRange(mergedText);
                    goodSplits.Clear();
                }

                IReadOnlyList<string> otherInfo = SplitText(s);
                finalChunks.AddRange(otherInfo);
            }
        }

        if (goodSplits.Count != 0)
        {
            var mergedText = MergeSplits(goodSplits, separator);
            finalChunks.AddRange(mergedText);
        }

        return finalChunks;
    }
}
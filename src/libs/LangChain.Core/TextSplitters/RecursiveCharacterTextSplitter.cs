using LangChain.Base;

namespace LangChain.TextSplitters;

/// <summary>
/// Implementation of splitting text that looks at characters.
/// Recursively tries to split by different characters to find one
/// that works.
/// </summary>
public class RecursiveCharacterTextSplitter : TextSplitter
{
    private readonly List<string> _separators;

    public RecursiveCharacterTextSplitter(List<string>? separators = null, int chunkSize = 4000, int chunkOverlap = 200, Func<string, int>? lengthFunction = null) : base(chunkSize, chunkOverlap, lengthFunction)
    {
        _separators = separators ?? new List<string> { "\n\n", "\n", " ", "" };
    }

    public override List<string> SplitText(string text)
    {
        List<string> finalChunks = new List<string>();
        string separator = _separators.Last();

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
            splits = text.Split(new string[] { separator }, StringSplitOptions.None).ToList();
        }
        else
        {
            splits = text.ToCharArray().Select(c => c.ToString()).ToList();
        }


        List<string> goodSplits = new List<string>();

        foreach (string s in splits)
        {
            if (s.Length < base.ChunkSize)
            {
                goodSplits.Add(s);
            }
            else
            {
                if (goodSplits.Count != 0)
                {
                    List<string> mergedText = MergeSplits(goodSplits, separator);
                    finalChunks.AddRange(mergedText);
                    goodSplits.Clear();
                }

                List<string> otherInfo = SplitText(s);
                finalChunks.AddRange(otherInfo);
            }
        }

        if (goodSplits.Count != 0)
        {
            List<string> mergedText = MergeSplits(goodSplits, separator);
            finalChunks.AddRange(mergedText);
        }

        return finalChunks;
    }
}
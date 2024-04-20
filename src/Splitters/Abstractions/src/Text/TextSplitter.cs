namespace LangChain.Splitters.Text;

/// <summary>
/// Functionality for splitting text.
/// <remarks>
/// - ported from langchain/text_splitter.py
/// 
/// </remarks>
/// </summary>
public abstract class TextSplitter : ITextSplitter
{
    private readonly int _chunkSize;
    private readonly int _chunkOverlap;
    private readonly Func<string, int> _lengthFunction;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chunkSize"></param>
    /// <param name="chunkOverlap"></param>
    /// <param name="lengthFunction"></param>
    /// <exception cref="ArgumentException"></exception>
    protected TextSplitter(
        int chunkSize = 4000,
        int chunkOverlap = 200,
        Func<string, int>? lengthFunction = null)
    {
        if (chunkOverlap > chunkSize)
        {
            throw new ArgumentException($"Chunk overlap ({chunkOverlap}) is greater than chunk size ({chunkSize}).");
        }

        _chunkSize = chunkSize;
        _chunkOverlap = chunkOverlap;
        _lengthFunction = lengthFunction ?? new Func<string, int>((str) => str.Length);
    }

    /// <summary>
    /// 
    /// </summary>
    protected int ChunkSize => _chunkSize;

    /// <summary>
    /// 
    /// </summary>
    protected int ChunkOverlap => _chunkOverlap;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public abstract IReadOnlyList<string> SplitText(string text);

    /// <summary>
    /// Joins a list of strings with a separator and returns null if the resulting string is empty
    /// </summary>
    protected string? JoinDocs(IReadOnlyList<string> docs, string separator)
    {
        var text = string.Join(separator, docs).Trim();
        return string.IsNullOrEmpty(text) ? null : text;
    }

    /// <summary>
    /// Merges a list of texts into chunks of size chunk_size with overlap
    /// </summary>
    protected IReadOnlyList<string> MergeSplits(IEnumerable<string> splits, string separator)
    {
        splits = splits ?? throw new ArgumentNullException(nameof(splits));
        
        var separatorLen = _lengthFunction(separator);
        var docs = new List<string>(); // result of chunks
        var currentDoc = new List<string>(); // documents of current chunk
        int total = 0;

        foreach (var split in splits)
        {
            int len = _lengthFunction(split);

            // if we can't fit the next split into current chunk
            if (total + len + (currentDoc.Count > 0 ? separatorLen : 0) >= _chunkSize)
            {
                // if the chunk is already was too big
                if (total > _chunkSize)
                {
                    // todo: Implement a logger
                    // todo: Log a warning about a split that is larger than the chunk size
                }


                if (currentDoc.Count > 0)
                {
                    // join all the docs in current chunk and add to the result
                    var doc = JoinDocs(currentDoc, separator);
                    if (doc != null)
                    {
                        docs.Add(doc);
                    }

                    // start erasing docs from the beginning of the chunk until we can fit the next split
                    while (total > _chunkOverlap || (total + len + (currentDoc.Count > 1 ? separatorLen : 0) > _chunkSize && total > 0))
                    {
                        total -= _lengthFunction(currentDoc[0]) + (currentDoc.Count > 1 ? separatorLen : 0);
                        currentDoc.RemoveAt(0);
                    }
                }
            }

            // add the next split to the current chunk
            currentDoc.Add(split);
            total += len + (currentDoc.Count > 1 ? separatorLen : 0); // recalculate the total length of the current chunk
        }

        // add the last chunk
        var lastDoc = JoinDocs(currentDoc, separator);
        if (lastDoc != null)
        {
            docs.Add(lastDoc);
        }

        return docs;
    }

    // todo: Implement from_huggingface_tokenizer
    // todo: Implement from_tiktoken_encoder


}
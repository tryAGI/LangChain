using LangChain.Docstore;

namespace LangChain.Base;

/// <summary>
/// Functionality for splitting text.
/// <remarks>
/// - ported from langchain/text_splitter.py
/// 
/// </remarks>
/// </summary>
public abstract class TextSplitter
{
    private readonly int _chunkSize;
    private readonly int _chunkOverlap;
    private readonly Func<string, int> _lengthFunction;



    protected TextSplitter(int chunkSize = 4000, int chunkOverlap = 200, Func<string, int>? lengthFunction = null)
    {
        if (chunkOverlap > chunkSize)
        {
            throw new ArgumentException($"Chunk overlap ({chunkOverlap}) is greater than chunk size ({chunkSize}).");
        }

        _chunkSize = chunkSize;
        _chunkOverlap = chunkOverlap;
        _lengthFunction = lengthFunction ?? new Func<string, int>((str) => str.Length);
    }

    protected int ChunkSize => _chunkSize;

    protected int ChunkOverlap => _chunkOverlap;

    public abstract List<string> SplitText(string text);

    /// <summary>
    /// Create documents from a list of texts.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// If the number of texts and metadata(when not null) are not equal, this method will throw an ArgumentException.
    /// </exception>
    public List<Document> CreateDocuments(List<string> texts, List<Dictionary<string, object>>? metadatas = null)
    {
        var documents = new List<Document>();

        // if no metadata is provided, create a list of empty dictionaries
        metadatas ??= Enumerable.Repeat(new Dictionary<string, object>(), texts.Count).ToList();

        if (texts.Count != metadatas.Count)
        {
            throw new ArgumentException("Number of texts and metadata must be equal.");
        }


        // each text is split into chunks, and each chunk is added to the list of documents
        for (int i = 0; i < texts.Count; i++)
        {
            var text = texts[i];
            var metadata = metadatas[i];

            foreach (var chunk in SplitText(text))
            {
                documents.Add(new Document(chunk, metadata));
            }
        }

        return documents;
    }

    public List<Document> SplitDocuments(IReadOnlyCollection<Document> documents)
    {
        var texts = documents.Select(doc => doc.PageContent).ToList();
        var metadatas = documents.Select(doc => doc.Metadata).ToList();

        return CreateDocuments(texts, metadatas);
    }

    /// <summary>
    /// Joins a list of strings with a separator and returns null if the resulting string is empty
    /// </summary>
    protected string? JoinDocs(List<string> docs, string separator)
    {
        var text = string.Join(separator, docs).Trim();
        return string.IsNullOrEmpty(text) ? null : text;
    }

    /// <summary>
    /// Merges a list of texts into chunks of size chunk_size with overlap
    /// </summary>
    protected List<string> MergeSplits(IEnumerable<string> splits, string separator)
    {
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
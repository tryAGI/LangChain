using LangChain.Docstore;
using LangChain.Splitters.Text;

namespace LangChain.Base;

/// <summary>
/// Functionality for splitting text.
/// <remarks>
/// - ported from langchain/text_splitter.py
/// 
/// </remarks>
/// </summary>
public static class TextSplitterExtensions
{
    /// <summary>
    /// Create documents from a list of texts.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// If the number of texts and metadata(when not null) are not equal, this method will throw an ArgumentException.
    /// </exception>
    public static List<Document> CreateDocuments(
        this TextSplitter splitter,
        List<string> texts,
        List<Dictionary<string, object>>? metadatas = null)
    {
        splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
        texts = texts ?? throw new ArgumentNullException(nameof(texts));
        
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

            foreach (var chunk in splitter.SplitText(text))
            {
                documents.Add(new Document(chunk, metadata));
            }
        }

        return documents;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="splitter"></param>
    /// <param name="documents"></param>
    /// <returns></returns>
    public static List<Document> SplitDocuments(
        this TextSplitter splitter,
        IReadOnlyCollection<Document> documents)
    {
        var texts = documents.Select(doc => doc.PageContent).ToList();
        var metadatas = documents.Select(doc => doc.Metadata).ToList();

        return splitter.CreateDocuments(texts, metadatas);
    }
}
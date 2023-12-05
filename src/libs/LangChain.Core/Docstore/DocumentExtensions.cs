namespace LangChain.Docstore;

/// <summary>
/// 
/// </summary>
public static class DocumentExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static Document ToDocument(this string self)
    {
        return new Document(self);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static List<Document> ToDocuments(this IEnumerable<string> self)
    {
        self = self ?? throw new ArgumentNullException(nameof(self));
        
        List<Document> documents = new();
        foreach (var item in self)
        {
            documents.Add(item.ToDocument());
        }
        return documents;
    }
}
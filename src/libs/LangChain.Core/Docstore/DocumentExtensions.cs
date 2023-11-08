using System.Runtime.CompilerServices;

namespace LangChain.Docstore;

public static class DocumentExtensions
{
    public static Document ToDocument(this string self)
    {
        return new Document(self);
    }

    public static List<Document> ToDocuments(this IEnumerable<string> self)
    {
        List<Document> documents = new();
        foreach (var item in self)
        {
            documents.Add(item.ToDocument());
        }
        return documents;
    }
}
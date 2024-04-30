using LangChain.Sources;

namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public static class VectorSearchResponseExtensions
{
    public static IReadOnlyList<Document> ToDocuments(
        this VectorSearchResponse vectorSearchResponse)
    {
        vectorSearchResponse = vectorSearchResponse ?? throw new ArgumentNullException(nameof(vectorSearchResponse));

        return vectorSearchResponse.Items
            .Select(static x => new Document(x.Text, x.Metadata))
            .ToArray();
    }
}
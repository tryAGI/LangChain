using LangChain.Databases;
using LangChain.DocumentLoaders;

namespace LangChain.Extensions;

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
namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="texts"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IReadOnlyList<Document> ToDocuments(this IEnumerable<string> texts)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));

        return texts
            .Select(text => new Document(text))
            .ToList();
    }
}
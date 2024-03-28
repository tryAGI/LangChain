using LangChain.Sources;

namespace LangChain.VectorStores;

/// <summary>
/// 
/// </summary>
public static class VectorStoreExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string AsString(
        this IEnumerable<Document> documents,
        string separator = "\n\n")
    {
        return string.Join(separator, documents.Select(x => x.PageContent));
    }
}
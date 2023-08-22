namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public static class DocumentExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public static IReadOnlyCollection<Document> AsArray(this Document document)
    {
        return new[]
        {
            document,
        };
    }
}
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
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IReadOnlyCollection<Document> AsArray(this Document document)
    {
        return new []
        {
            document,
        };
    }
}
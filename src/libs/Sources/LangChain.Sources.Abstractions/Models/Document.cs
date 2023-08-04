namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
/// <param name="Content"></param>
/// <param name="Metadata"></param>
public readonly record struct Document(
    string Content,
    IReadOnlyDictionary<string, string> Metadata)
{
    /// <summary>
    /// 
    /// </summary>
    public static Document Empty { get; } = new(
        Content: string.Empty,
        Metadata: new Dictionary<string, string>());
}
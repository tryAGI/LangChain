namespace LangChain.Splitters;

/// <summary>
/// Represents a part of a document.
/// </summary>
/// <param name="Name"></param>
/// <param name="Content"></param>
/// <param name="Type"></param>
public readonly record struct DocumentPart(
    string Name,
    string Content,
    DocumentPartType Type = DocumentPartType.Unknown)
{
    /// <summary>
    /// Represents an empty document part.
    /// </summary>
    public static DocumentPart Empty { get; } = new(
        Name: string.Empty,
        Content: string.Empty,
        Type: DocumentPartType.Unknown);
}
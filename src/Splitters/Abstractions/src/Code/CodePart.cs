namespace LangChain.Splitters.Code;

/// <summary>
/// Represents a part of a document.
/// </summary>
/// <param name="Name"></param>
/// <param name="Content"></param>
/// <param name="Type"></param>
public readonly record struct CodePart(
    string Name,
    string Content,
    CodePartType Type = CodePartType.Unknown)
{
    /// <summary>
    /// Represents an empty document part.
    /// </summary>
    public static CodePart Empty { get; } = new(
        Name: string.Empty,
        Content: string.Empty,
        Type: CodePartType.Unknown);
}
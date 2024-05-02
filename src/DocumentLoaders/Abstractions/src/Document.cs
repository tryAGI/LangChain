namespace LangChain.DocumentLoaders;

/// <summary>
/// Class for storing document
/// <remarks>
/// - no BaseModel implementation from pydantic
/// - ported from langchain/docstore/document.py
/// </remarks>
/// </summary>
public class Document
{
    /// <summary>
    /// 
    /// </summary>
    public Document()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="metadata"></param>
    public Document(string content, IReadOnlyDictionary<string, object>? metadata = null)
    {
        PageContent = content;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// 
    /// </summary>
    public static Document Empty { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    public string PageContent { get; init; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = new Dictionary<string, object>();

    private static readonly string[] Separator = ["\n\n"];

    /// <summary>
    /// Paragraphs of the page.
    /// </summary>
    public IReadOnlyList<string> Paragraphs()
    {
        return PageContent.Split(Separator, StringSplitOptions.None).ToList();
    }

    /// <summary>
    /// Summary of the page (the first paragraph)
    /// </summary>
    public string Summary()
    {
        return Paragraphs()[0];
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var serializedMetadata = string.Join(", ", Metadata.Select(x => $"{{{x.Key}:{x.Value}}}"));
        return $"(PageContent='{PageContent}', Metadata={serializedMetadata})";
    }

}
using System.Globalization;

namespace LangChain.Docstore;

/// <summary>
/// Class for storing document
/// <remarks>
/// - no BaseModel implementation from pydantic
/// - ported from langchain/docstore/document.py
/// </remarks>
/// </summary>
public class Document
{
    public Document(string content, Dictionary<string, object>? metadata = null)
    {
        metadata ??= new Dictionary<string, object>();
        PageContent = content;
        Metadata = metadata;
    }

    public static Document Empty { get; } = new(
        content: string.Empty,
        metadata: new Dictionary<string, object>());

    public string PageContent { get; set; }
    public int LookupIndex { get; set; }
    public string LookupStr { get; set; }
    public Dictionary<string, object> Metadata { get; set; }

    private static readonly string[] separator = { "\n\n" };

    /// <summary>
    /// Paragraphs of the page.
    /// </summary>
    public List<string> Paragraphs()
    {
        return PageContent.Split(separator, StringSplitOptions.None).ToList();
    }
    /// <summary>
    /// Summary of the page (the first paragraph)
    /// </summary>
    public string Summary()
    {
        return Paragraphs()[0];
    }

    /// <summary>
    /// Lookup a term in the page, imitating cmd-F functionality.
    /// </summary>
    public string Lookup(string searchString)
    {
        // if there is a new search string, reset the index
        if (!searchString.Equals(LookupStr, StringComparison.OrdinalIgnoreCase))
        {
            LookupStr = searchString.ToLower(CultureInfo.InvariantCulture);
            LookupIndex = 0;
        }
        else
        {
            LookupIndex++;
        }

        // get all the paragraphs that contain the search string
        var lookups = Paragraphs()
            .Where(p => p.ToLower(CultureInfo.InvariantCulture).Contains(LookupStr))
            .ToList();
        if (lookups.Count == 0)
        {
            return "No Results";
        }
        else if (LookupIndex >= lookups.Count)
        {
            return "No More Results";
        }
        else
        {
            string resultPrefix = $"(Result {LookupIndex + 1}/{lookups.Count})";
            return $"{resultPrefix} {lookups[LookupIndex]}";
        }
    }

    public override string ToString()
    {
        var serializedMetadata = string.Join(", ", Metadata.Select(x => $"{{{x.Key}:{x.Value}}}"));
        return $"(PageContent='{PageContent}', LookupStr='{LookupStr}', Metadata={serializedMetadata}), LookupIndex={LookupIndex}";
    }

}
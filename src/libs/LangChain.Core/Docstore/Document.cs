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
    public Document(string content, Dictionary<string, object>? metadata = null)
    {
        PageContent = content;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// 
    /// </summary>
    public static Document Empty { get; } = new(
        content: string.Empty,
        metadata: new Dictionary<string, object>());

    /// <summary>
    /// 
    /// </summary>
    public string PageContent { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    public int LookupIndex { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string LookupStr { get; set; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    private readonly static string[] separator = { "\n\n" };

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
        searchString = searchString ?? throw new ArgumentNullException(nameof(searchString));
        
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

    /// <inheritdoc/>
    public override string ToString()
    {
        var serializedMetadata = string.Join(", ", Metadata.Select(x => $"{{{x.Key}:{x.Value}}}"));
        return $"(PageContent='{PageContent}', LookupStr='{LookupStr}', Metadata={serializedMetadata}), LookupIndex={LookupIndex}";
    }

}
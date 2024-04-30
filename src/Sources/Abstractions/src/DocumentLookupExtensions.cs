namespace LangChain.Sources;

public static class DocumentLookupExtensions
{
    /// <summary>
    /// Lookup a term in the page, imitating cmd-F functionality.
    /// </summary>
    public static IEnumerable<string> Lookup(this Document document, string searchString)
    {
        document = document ?? throw new ArgumentNullException(nameof(document));
        searchString = searchString ?? throw new ArgumentNullException(nameof(searchString));

        foreach (var lookup in document.Paragraphs()
            .Where(p => p.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
        {
            yield return lookup;
        }
    }
}
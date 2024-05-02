using AngleSharp;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public class HtmlLoader : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(DataSource dataSource, CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        
        if (dataSource.Type != DataSourceType.Uri)
        {
            throw new NotSupportedException("Only Uri is supported");
        }
        
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(dataSource.Value!, cancellation: cancellationToken).ConfigureAwait(false);

        foreach (var element in document.QuerySelectorAll("script, style, meta, link"))
        {
            element.Remove();
        }

        var html =
            document.QuerySelector("html") ??
            throw new NotSupportedException("Not supported for pages without <html> tag");
        
        return new Document[] { new(html.TextContent, new Dictionary<string, object>
        {
            { "url", dataSource.Value! },
        }) };
    }
}
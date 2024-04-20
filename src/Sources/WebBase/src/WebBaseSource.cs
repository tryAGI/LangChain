using AngleSharp;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public class WebBaseSource : ISource
{
    /// <summary>
    /// 
    /// </summary>
    public required string Url { get; init; }

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<Document>> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return LoadCoreAsync(Url);
        }
        catch (Exception exception)
        {
            return Task.FromException<IReadOnlyCollection<Document>>(exception);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    protected async Task<IReadOnlyCollection<Document>> LoadCoreAsync(string url)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url).ConfigureAwait(false);

        foreach (var element in document.QuerySelectorAll("script, style, meta, link"))
        {
            element.Remove();
        }

        string content;
        var html = document.QuerySelector("html");

        if (html == null)
        {
            throw new NotSupportedException("Not supported for pages without <html> tag");
        }

        content = html.TextContent;

        var documents = new Document[] { new(content, new Dictionary<string, object> { { "url", url } }) };

        return documents;
    }
}
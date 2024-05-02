using UglyToad.PdfPig;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed class PdfPigPdfLoader : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(DataSource dataSource,
        CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(paramName: nameof(dataSource));

        using var stream = await dataSource.GetStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = PdfDocument.Open(stream, new ParsingOptions());

        return document
            .GetPages()
            .Select(page => new Document(page.Text, new Dictionary<string, object>
            {
                { "path", dataSource.Value ?? string.Empty },
                { "type", dataSource.Type.ToString() },
                { "page", page.Number },
            }))
            .ToArray();
    }
}
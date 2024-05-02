using UglyToad.PdfPig;

namespace LangChain.DocumentLoaders;

/// <summary>
/// 
/// </summary>
public sealed class PdfPigPdfLoader : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(
        DataSource dataSource,
        DocumentLoaderSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(paramName: nameof(dataSource));

        using var stream = await dataSource.GetStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = PdfDocument.Open(stream, new ParsingOptions());

        var metadata = settings.CollectMetadata(dataSource);

        return document
            .GetPages()
            .Select(page => new Document(page.Text, metadata.With(new Dictionary<string, object>
            {
                { "page", page.Number },
            })))
            .ToArray();
    }
}
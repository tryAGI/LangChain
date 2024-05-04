using Aspose.Pdf.Text;

namespace LangChain.DocumentLoaders;

/// <summary>
/// 
/// </summary>
public sealed class AsposePdfLoader : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(
        DataSource dataSource,
        DocumentLoaderSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(paramName: nameof(dataSource));

        using var stream = await dataSource.GetStreamAsync(cancellationToken).ConfigureAwait(false);
        using var pdfDocument = new Aspose.Pdf.Document(stream);
        var textAbsorber = new TextAbsorber();
        pdfDocument.Pages.Accept(textAbsorber);

        var metadata = settings.CollectMetadataIfRequired(dataSource);

        return [new Document(textAbsorber.Text, metadata: metadata)];
    }
}
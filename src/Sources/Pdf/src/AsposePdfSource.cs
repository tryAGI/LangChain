using Aspose.Pdf.Text;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed class AsposePdfLoader : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(DataSource dataSource, CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(paramName: nameof(dataSource));

        using var stream = await dataSource.GetStreamAsync(cancellationToken).ConfigureAwait(false);
        using var pdfDocument = new Aspose.Pdf.Document(stream);
        var textAbsorber = new TextAbsorber();
        pdfDocument.Pages.Accept(textAbsorber);

        var documents = new Document[]
        {
            new(textAbsorber.Text, new Dictionary<string, object>
            {
                { "path", dataSource.Value ?? string.Empty },
                { "type", dataSource.Type.ToString() },
            })
        };
        return documents;
    }
}
using Aspose.Pdf.Text;
using LangChain.Base;
using Document = LangChain.Docstore.Document;
namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public class AsposePdfSource : ISource
{
    /// <summary>
    /// 
    /// </summary>
    public required string Path { get; init; }

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<Document>> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var pdfDocument = new Aspose.Pdf.Document(Path);
            var textAbsorber = new TextAbsorber();
            pdfDocument.Pages.Accept(textAbsorber);

            var documents = new Document[] { new(textAbsorber.Text, new Dictionary<string, object> { { "path", Path } }) };
            return Task.FromResult<IReadOnlyCollection<Document>>(documents);
        }
        catch (Exception exception)
        {
            return Task.FromException<IReadOnlyCollection<Document>>(exception);
        }
    }
}
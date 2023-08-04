using Aspose.Pdf.Text;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public class PdfSource : ISource
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
        
            var documents = (Document.Empty with
            {
                Content = textAbsorber.Text,
            }).AsArray();

            return Task.FromResult(documents);
        }
        catch (Exception exception)
        {
            return Task.FromException<IReadOnlyCollection<Document>>(exception);
        }
    }
}
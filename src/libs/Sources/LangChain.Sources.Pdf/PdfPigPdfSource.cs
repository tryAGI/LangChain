using LangChain.Base;
using UglyToad.PdfPig;
using Document=LangChain.Docstore.Document;
namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public class PdfPigPdfSource : ISource
{

    public string Path { get; }

    public PdfPigPdfSource(string path)
    {
        Path = path;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<Document>> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using PdfDocument document = PdfDocument.Open(Path, new ParsingOptions());
            var pages = document.GetPages();


            var documents = pages.Select(page => new Document(page.Text, new Dictionary<string, object>
            {
                {"path",Path},
                {"page",page.Number}
  

            })).ToArray();

            return Task.FromResult<IReadOnlyCollection<Document>>(documents);
        }
        catch (Exception exception)
        {
            return Task.FromException<IReadOnlyCollection<Document>>(exception);
        }
    }
}
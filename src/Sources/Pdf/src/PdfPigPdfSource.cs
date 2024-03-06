using UglyToad.PdfPig;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed partial class PdfPigPdfSource : ISource, IDisposable
#if NET7_0_OR_GREATER
    , ICreatableFromStream<PdfPigPdfSource>
#endif
{
    public static PdfPigPdfSource CreateFromStream(Stream stream)
    {
        return new PdfPigPdfSource(stream);
    }

    /// <summary>
    /// 
    /// </summary>
    public string Path { get; } = string.Empty;
    
    /// <summary>
    /// 
    /// </summary>
    public Stream? Stream { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public PdfPigPdfSource(string path)
    {
        Path = path;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public PdfPigPdfSource(Stream stream)
    {
        Stream = stream;
    }
    
    /// <inheritdoc/>
    public Task<IReadOnlyCollection<Document>> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var document = Stream != null
                ? PdfDocument.Open(Stream, new ParsingOptions())
                : PdfDocument.Open(Path, new ParsingOptions());
            var pages = document.GetPages();
            var documents = pages
                .Select(page => new Document(page.Text, new Dictionary<string, object>
                {
                    {"path",Path},
                    {"page",page.Number},
                }))
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Document>>(documents);
        }
        catch (Exception exception)
        {
            return Task.FromException<IReadOnlyCollection<Document>>(exception);
        }
    }

    public void Dispose()
    {
        Stream?.Dispose();
    }
}
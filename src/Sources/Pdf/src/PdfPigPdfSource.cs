using System.Net.Http;
using UglyToad.PdfPig;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed class PdfPigPdfSource : ISource, IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyCollection<Document>> FromStreamAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        using var source = new PdfPigPdfSource(stream);
        
        return await source.LoadAsync(cancellationToken).ConfigureAwait(false);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyCollection<Document>> DocumentsFromUriAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        using var source = await CreateFromUriAsync(uri, cancellationToken).ConfigureAwait(false);
        
        return await source.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<PdfPigPdfSource> CreateFromUriAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        using var client = new HttpClient();
        using var stream = await client.GetStreamAsync(uri).ConfigureAwait(false);

        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        memoryStream.Position = 0;

        return new PdfPigPdfSource(memoryStream);
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
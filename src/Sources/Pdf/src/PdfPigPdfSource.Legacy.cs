namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed partial class PdfPigPdfSource
{
    public static async Task<IReadOnlyCollection<Document>> LoadDocumentsFromStreamAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        using var source = new PdfPigPdfSource(stream);

        return await source.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<IReadOnlyCollection<Document>> LoadDocumentsFromUriAsync(
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
        var memoryStream = await uri.DownloadAsMemoryStreamAsync(cancellationToken).ConfigureAwait(false);

        return CreateFromStream(memoryStream);
    }
}
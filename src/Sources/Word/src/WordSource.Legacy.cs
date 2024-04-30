namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed partial class WordSource
{
    public static async Task<IReadOnlyCollection<Document>> LoadDocumentsFromStreamAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var source = new WordSource(stream);

        return await source.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<IReadOnlyCollection<Document>> LoadDocumentsFromUriAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var source = await CreateFromUriAsync(uri, cancellationToken).ConfigureAwait(false);

        return await source.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<WordSource> CreateFromUriAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = await uri.DownloadAsMemoryStreamAsync(cancellationToken).ConfigureAwait(false);

        return CreateFromStream(memoryStream);
    }
}
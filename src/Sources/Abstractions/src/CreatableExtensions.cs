using System.Net.Http;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public static class CreatableExtensions
{
    public static async Task<MemoryStream> DownloadAsMemoryStreamAsync(
        this Uri uri,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        using var client = new HttpClient();
        using var stream = await client.GetStreamAsync(uri, cancellationToken).ConfigureAwait(false);

        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        memoryStream.Position = 0;

        return memoryStream;
    }

#if NET7_0_OR_GREATER
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyCollection<Document>> LoadDocumentsAsync<TSource>(
        this Stream stream,
        CancellationToken cancellationToken = default)
        where TSource : class, ICreatableFromStream<TSource>, ISource
    {
        TSource? source = null;
        try
        {
            source = TSource.CreateFromStream(stream);

            return await source.LoadAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (source is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyCollection<Document>> LoadDocumentsAsync<TSource>(
        this Uri uri,
        CancellationToken cancellationToken = default)
        where TSource : class, ICreatableFromStream<TSource>, ISource
    {
        TSource? source = null;
        try
        {
            source = await uri.CreateAsync<TSource>(cancellationToken).ConfigureAwait(false);

            return await source.LoadAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (source is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public static async Task<T> CreateAsync<T>(
        this Uri uri,
        CancellationToken cancellationToken = default)
        where T : ICreatableFromStream<T>
    {
        var memoryStream = await uri.DownloadAsMemoryStreamAsync(cancellationToken).ConfigureAwait(false);

        return T.CreateFromStream(memoryStream);
    }
    
#endif
}
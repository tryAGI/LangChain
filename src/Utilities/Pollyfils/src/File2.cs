// ReSharper disable once CheckNamespace
namespace System.IO;

public static class File2
{
    public static
#if NET6_0_OR_GREATER
        async
#endif
        Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
#if NET6_0_OR_GREATER
        await File.WriteAllBytesAsync(path, bytes, cancellationToken).ConfigureAwait(false);
#else
        try
        {
            File.WriteAllBytes(path, bytes);

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
#endif
    }
    
    
    public static
#if NET6_0_OR_GREATER
        async
#endif
        Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default)
    {
#if NET6_0_OR_GREATER
        await File.WriteAllTextAsync(path, contents, cancellationToken).ConfigureAwait(false);
#else
        try
        {
            File.WriteAllText(path, contents);

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
#endif
    }
}
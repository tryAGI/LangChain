// ReSharper disable once CheckNamespace

using System.Text;

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
    
    public static
#if NET6_0_OR_GREATER
        async
#endif
        Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
#if NET6_0_OR_GREATER
        return await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
#else
        try
        {
            var text = File.ReadAllText(path);

            return Task.FromResult(text);
        }
        catch (Exception exception)
        {
            return Task.FromException<string>(exception);
        }
#endif
    }
    
    public static
#if NET6_0_OR_GREATER
        async
#endif
        Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
    {
#if NET6_0_OR_GREATER
        return await File.ReadAllTextAsync(path, encoding, cancellationToken).ConfigureAwait(false);
#else
        try
        {
            var text = File.ReadAllText(path, encoding);

            return Task.FromResult(text);
        }
        catch (Exception exception)
        {
            return Task.FromException<string>(exception);
        }
#endif
    }
}
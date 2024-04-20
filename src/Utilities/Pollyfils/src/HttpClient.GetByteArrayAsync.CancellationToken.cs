#if !NET6_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static partial class HttpClientExtensions
{
    public static async Task<byte[]> GetByteArrayAsync(
        this HttpClient client,
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        client = client ?? throw new ArgumentNullException(nameof(client));
        
        return await client.GetByteArrayAsync(uri).ConfigureAwait(false);
    }
}
#endif
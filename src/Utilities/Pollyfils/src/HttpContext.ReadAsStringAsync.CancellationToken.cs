// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static partial class HttpContextExtensions
{
#if !NET6_0_OR_GREATER
    public static async Task<string> ReadAsStringAsync(
        this HttpContent content,
        CancellationToken cancellationToken = default)
    {
        content = content ?? throw new ArgumentNullException(nameof(content));
        
        return await content.ReadAsStringAsync().ConfigureAwait(false);
    }
#endif
}
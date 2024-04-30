#if !NET6_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static partial class HttpContextExtensions
{
    public static async Task<string> ReadAsStringAsync(
        this HttpContent content,
        CancellationToken cancellationToken = default)
    {
        content = content ?? throw new ArgumentNullException(nameof(content));

        return await content.ReadAsStringAsync().ConfigureAwait(false);
    }
}
#endif
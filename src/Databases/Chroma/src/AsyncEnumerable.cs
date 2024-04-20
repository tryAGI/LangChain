namespace LangChain.Databases.Chroma;

/// <summary>
/// 
/// </summary>
public static class AsyncEnumerable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
    {
        var result = new List<T>();

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            result.Add(item);
        }

        return result;
    }
}
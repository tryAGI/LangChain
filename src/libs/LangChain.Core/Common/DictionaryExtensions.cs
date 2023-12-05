namespace LangChain.Common;

public static class DictionaryExtensions
{
    /// <summary>
    /// Adds key values of additional to target dictionary if key is not yet present in target
    /// </summary>
    /// <returns>target dictionary</returns>
    public static void TryAddKeyValues<TKey, TValue>(
        this Dictionary<TKey, TValue> target,
        IReadOnlyDictionary<TKey, TValue> additional)
        where TKey : notnull
    {
        target = target ?? throw new ArgumentNullException(nameof(target));
        additional = additional ?? throw new ArgumentNullException(nameof(additional));

        foreach (var kv in additional)
        {
            if (!target.ContainsKey(kv.Key))
            {
                target.Add(kv.Key, kv.Value);
            }
        }
    }
}
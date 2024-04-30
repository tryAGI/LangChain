#if !NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace

namespace System;

public static partial class PolyfillDictionaryExtensions
{
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
    {
        dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
}
#endif
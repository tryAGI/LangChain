#if !NET6_0_OR_GREATER
// ReSharper disable once CheckNamespace

namespace System;

public static partial class PolyfillStringExtensions
{
    public static bool StartsWith(this string text, char value)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));
        
        return text.StartsWith(value.ToString(), StringComparison.Ordinal);
    }
}
#endif
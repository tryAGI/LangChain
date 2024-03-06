// ReSharper disable once CheckNamespace

namespace System;

public static class PolyfillStringExtensions
{
    public static bool Contains(this string text, string value, StringComparison comparisonType)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));
        
#if NET6_0_OR_GREATER
        return text.Contains(value, comparisonType);
#else
        switch (comparisonType)
        {
            case StringComparison.CurrentCulture:
                return text.IndexOf(value, StringComparison.CurrentCulture) >= 0;
            case StringComparison.CurrentCultureIgnoreCase:
                return text.IndexOf(value, StringComparison.CurrentCultureIgnoreCase) >= 0;
            case StringComparison.InvariantCulture:
                return text.IndexOf(value, StringComparison.InvariantCulture) >= 0;
            case StringComparison.InvariantCultureIgnoreCase:
                return text.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0;
            case StringComparison.Ordinal:
                return text.IndexOf(value, StringComparison.Ordinal) >= 0;
            case StringComparison.OrdinalIgnoreCase:
                return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
            default:
                throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null);
        }
#endif
    }
}
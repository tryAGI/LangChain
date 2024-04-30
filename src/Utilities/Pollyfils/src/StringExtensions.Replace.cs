#if !NET6_0_OR_GREATER
// ReSharper disable once CheckNamespace

namespace System;

public static partial class PolyfillStringExtensions
{
    public static string Replace(this string text, string from, string to, StringComparison comparisonType)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));
        from = from ?? throw new ArgumentNullException(nameof(from));
        to = to ?? throw new ArgumentNullException(nameof(to));

        switch (comparisonType)
        {
            case StringComparison.CurrentCulture:
            case StringComparison.InvariantCulture:
            case StringComparison.Ordinal:
                return text.Replace(from, to);

            case StringComparison.InvariantCultureIgnoreCase:
            case StringComparison.OrdinalIgnoreCase:
            case StringComparison.CurrentCultureIgnoreCase:
                return text
                    .Replace(from.ToLowerInvariant(), to)
                    .Replace(from.ToUpperInvariant(), to)
                    .Replace(from, to);
            default:
                throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null);
        }
    }
}
#endif
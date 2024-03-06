#if !NET6_0_OR_GREATER
// ReSharper disable once CheckNamespace

namespace System;

public static partial class PolyfillStreamExtensions
{
    public static Task CopyToAsync(
        this Stream stream,
        Stream destination,
        CancellationToken cancellationToken)
    {
        stream = stream ?? throw new ArgumentNullException(nameof(stream));
        
        return stream.CopyToAsync(destination, bufferSize: 81920, cancellationToken);
    }
}
#endif
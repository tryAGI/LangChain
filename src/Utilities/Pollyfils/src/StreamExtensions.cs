// ReSharper disable once CheckNamespace

namespace System;

public static class PolyfillStreamExtensions
{
    public static Task CopyToAsync(
        this Stream stream,
        Stream destination,
        CancellationToken cancellationToken)
    {
        stream = stream ?? throw new ArgumentNullException(nameof(stream));
        
#if NET6_0_OR_GREATER
        return stream.CopyToAsync(destination, cancellationToken);
#else
        return stream.CopyToAsync(destination, bufferSize: 81920, cancellationToken);
#endif
    }
}
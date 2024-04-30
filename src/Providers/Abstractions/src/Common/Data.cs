// ReSharper disable once CheckNamespace

namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class Data
{
    private byte[]? _bytes;
    private string? _base64;

    /// <inheritdoc cref="ToData(string)"/>
    public static implicit operator Data(string message)
    {
        return ToData(message);
    }

    /// <summary>
    /// Explicitly converts a string to a <see cref="Data"/>. <br/>
    /// Will be converted to a <see cref="Data"/>
    /// with non null Base64 property.
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static Data ToData(string base64)
    {
        return new Data
        {
            _base64 = base64,
        };
    }

    public static Data FromStream(Stream stream)
    {
        stream = stream ?? throw new ArgumentNullException(nameof(stream));

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return new Data
        {
            _bytes = memoryStream.ToArray(),
        };
    }

    public static Data FromBytes(byte[] bytes)
    {
        return new Data
        {
            _bytes = bytes,
        };
    }

    public static Data FromBase64(string base64)
    {
        return new Data
        {
            _base64 = base64,
        };
    }

    public static implicit operator byte[](Data file)
    {
        return file?.ToByteArray() ?? [];
    }

    public static implicit operator string(Data file)
    {
        return file?.ToBase64() ?? string.Empty;
    }

    public static implicit operator Stream(Data file)
    {
        return file?.ToStream() ?? new MemoryStream();
    }

    public string ToBase64()
    {
        _base64 ??= Convert.ToBase64String(_bytes ?? Array.Empty<byte>());

        return _base64;
    }

    public byte[] ToByteArray()
    {
        _bytes ??= Convert.FromBase64String(_base64 ?? string.Empty);

        return _bytes;
    }

    public MemoryStream ToStream()
    {
        return new MemoryStream(ToByteArray());
    }

    public override string ToString()
    {
        return ToBase64();
    }
}
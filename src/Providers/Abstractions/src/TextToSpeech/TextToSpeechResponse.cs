// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class TextToSpeechResponse
{
    public required byte[] Bytes { get; init; }

    public Usage Usage { get; init; } = Usage.Empty;

    public void Deconstruct(
        out byte[] values,
        out Usage usage)
    {
        values = Bytes;
        usage = Usage;
    }

    public static implicit operator byte[](TextToSpeechResponse response)
    {
        return response?.ToByteArray() ?? [];
    }

    public static implicit operator Stream(TextToSpeechResponse response)
    {
        return response?.ToStream() ?? Stream.Null;
    }

    public byte[] ToByteArray()
    {
        return Bytes;
    }

    public Stream ToStream()
    {
        return new MemoryStream(Bytes);
    }
}
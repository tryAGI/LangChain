// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class TextToImageResponse
{
    public IReadOnlyList<Data> Images { get; init; } = new List<Data>();

    public Usage Usage { get; init; } = Usage.Empty;

    /// <summary>
    /// 
    /// </summary>
    public required TextToImageSettings UsedSettings { get; init; }

    public void Deconstruct(
        out byte[] values,
        out Usage usage)
    {
        values = ToByteArray();
        usage = Usage;
    }

    public void Deconstruct(
        out byte[] values,
        out Usage usage,
        out TextToImageSettings usedSettings)
    {
        values = ToByteArray();
        usage = Usage;
        usedSettings = UsedSettings;
    }

    public static implicit operator byte[](TextToImageResponse response)
    {
        return response?.ToByteArray() ?? [];
    }

    public byte[] ToByteArray()
    {
        return Images.ElementAtOrDefault(0)?.ToByteArray() ?? Array.Empty<byte>();
    }
}
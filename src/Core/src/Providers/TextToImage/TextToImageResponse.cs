#pragma warning disable CA2225

namespace LangChain.Providers;

/// <summary>
/// Text-to-image response.
/// </summary>
public class TextToImageResponse
{
    public IReadOnlyList<Data> Images { get; init; } = new List<Data>();

    public Usage Usage { get; init; } = Usage.Empty;

    /// <summary>
    /// The settings used for this request.
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

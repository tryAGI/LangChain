// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class TextToImageResponse
{
    public required byte[] Bytes { get; init; }
    
    public Usage Usage { get; init; } = Usage.Empty;
    
    public MemoryStream AsStream()
    {
        return new MemoryStream(Bytes);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public required TextToImageSettings UsedSettings { get; init; }
    
    public void Deconstruct(
        out byte[] values,
        out Usage usage)
    {
        values = Bytes;
        usage = Usage;
    }
    
    public void Deconstruct(
        out byte[] values,
        out Usage usage,
        out TextToImageSettings usedSettings)
    {
        values = Bytes;
        usage = Usage;
        usedSettings = UsedSettings;
    }
    
    public static implicit operator byte[](TextToImageResponse response)
    {
        return response?.ToByteArray() ?? [];
    }

    public byte[] ToByteArray()
    {
        return Bytes;
    }
}
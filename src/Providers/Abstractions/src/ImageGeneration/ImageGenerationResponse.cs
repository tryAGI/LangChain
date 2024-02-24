// ReSharper disable once CheckNamespace
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class ImageGenerationResponse
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
    public required ImageGenerationSettings UsedSettings { get; init; }
    
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
        out ImageGenerationSettings usedSettings)
    {
        values = Bytes;
        usage = Usage;
        usedSettings = UsedSettings;
    }
    
    public static implicit operator byte[](ImageGenerationResponse response)
    {
        return response?.ToByteArray() ?? [];
    }

    public byte[] ToByteArray()
    {
        return Bytes;
    }
}
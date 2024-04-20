// ReSharper disable once CheckNamespace
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class EmbeddingResponse
{
    /// <summary>
    /// 
    /// </summary>
    public required float[][] Values { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public required EmbeddingSettings UsedSettings { get; init; }
    
    public Usage Usage { get; init; } = Usage.Empty;
    
    public void Deconstruct(
        out float[] values,
        out Usage usage)
    {
        values = Values.FirstOrDefault() ?? [];
        usage = Usage;
    }
    
    public void Deconstruct(
        out float[] values,
        out Usage usage,
        out EmbeddingSettings usedSettings)
    {
        values = Values.FirstOrDefault() ?? [];
        usage = Usage;
        usedSettings = UsedSettings;
    }
    
    public static implicit operator float[](EmbeddingResponse response)
    {
        return response?.ToSingleArray() ?? [];
    }

    public static implicit operator float[][](EmbeddingResponse response)
    {
        return response?.Values ?? [];
    }
    
    public float[] ToSingleArray()
    {
        return Values.FirstOrDefault() ?? [];
    }
}
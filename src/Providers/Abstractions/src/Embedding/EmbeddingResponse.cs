// ReSharper disable once CheckNamespace
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace LangChain.Providers;

/// <summary>
/// The response containing the embeddings.
/// </summary>
public class EmbeddingResponse
{
    /// <summary>
    /// The embeddings.
    /// </summary>
    public required float[][] Values { get; init; }

    /// <summary>
    /// Used settings for the embeddings.
    /// </summary>
    public required EmbeddingSettings UsedSettings { get; init; }

    /// <summary>
    /// Dimensions of the embeddings.
    /// </summary>
    public int Dimensions { get; init; }

    /// <summary>
    /// Usage information.
    /// </summary>
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
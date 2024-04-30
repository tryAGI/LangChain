namespace LangChain.Databases;

/// <summary>
/// Vector search request.
/// </summary>
public class VectorSearchRequest
{
    /// <summary>
    /// Embedding to look up documents similar to.
    /// </summary>
    public required float[][] Embeddings { get; set; }

    public static implicit operator VectorSearchRequest(float[] embedding)
    {
        return ToVectorSearchRequest(embedding);
    }

    public static implicit operator VectorSearchRequest(float[][] embeddings)
    {
        return ToVectorSearchRequest(embeddings);
    }

    public static VectorSearchRequest ToVectorSearchRequest(float[] embedding)
    {
        return ToVectorSearchRequest([embedding]);
    }

    public static VectorSearchRequest ToVectorSearchRequest(float[][] embeddings)
    {
        return new VectorSearchRequest
        {
            Embeddings = embeddings,
        };
    }
}
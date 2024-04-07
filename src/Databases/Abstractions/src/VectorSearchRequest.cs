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
}
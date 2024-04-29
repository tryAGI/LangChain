namespace LangChain.Databases;

/// <summary>
/// Vector search response.
/// </summary>
public class VectorSearchResponse
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<Vector> Items { get; set; } = [];
}
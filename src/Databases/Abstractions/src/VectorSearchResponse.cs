namespace LangChain.Databases;

/// <summary>
/// Vector search response.
/// </summary>
public class VectorSearchResponse
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<VectorSearchItem> Items { get; set; } = [];
}
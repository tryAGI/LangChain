namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public enum DistanceStrategy
{
    /// <summary>
    /// Euclidean distance (L2 distance)
    /// </summary>
    Euclidean,

    /// <summary>
    /// Cosine distance
    /// </summary>
    Cosine,

    /// <summary>
    /// Inner product
    /// </summary>
    InnerProduct
}
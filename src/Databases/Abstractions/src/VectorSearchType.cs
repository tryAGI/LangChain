namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public enum VectorSearchType
{
    /// <summary>
    /// 
    /// </summary>
    Similarity,

    /// <summary>
    /// 
    /// </summary>
    SimilarityScoreThreshold,

    /// <summary>
    /// Maximal marginal relevance optimizes for similarity to query and diversity among selected documents.
    /// </summary>
    MaximumMarginalRelevance,
}
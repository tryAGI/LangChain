namespace LangChain.Databases;

/// <summary>
/// Vector search settings.
/// </summary>
public class VectorSearchSettings
{
    /// <summary>
    /// 
    /// </summary>
    public VectorSearchType Type { get; set; } = VectorSearchType.Similarity;

    /// <summary>
    /// Number of Documents to return. Defaults to 4.
    /// </summary>
    public int NumberOfResults { get; set; } = 4;

    /// <summary>
    /// Number of Documents to fetch to pass to MMR algorithm.
    /// </summary>
    public int FetchK { get; set; } = 20;

    /// <summary>
    /// Applied only to the MMR algorithm.
    /// Number between 0 and 1 that determines the degree
    /// of diversity among the results with 0 corresponding
    /// to maximum diversity and 1 to minimum diversity. <br/>
    /// Defaults to 0.5.
    /// </summary>
    public float LambdaMult { get; set; } = 0.5f;

    public DistanceStrategy DistanceStrategy { get; set; } = DistanceStrategy.Cosine;

    /// <summary>
    /// The 'correct' relevance function
    /// may differ depending on a few things, including:
    /// - the distance / similarity metric used by the VectorStore
    /// - the scale of your embeddings (OpenAI's are unit normed. Many others are not!)
    /// - embedding dimensionality
    /// - etc.
    /// </summary>
    public Func<float, float>? RelevanceScoreFunc { get; set; }

    /// <summary>
    /// A floating point value between 0 to 1 to filter the resulting set of retrieved docs.
    /// Will only return docs with a similarity score greater or equal than this value.
    /// Ignored by default.
    /// </summary>
    public float? ScoreThreshold { get; set; }

    public VectorSearchSettings()
    {
        RelevanceScoreFunc = RelevanceScoreFunctions.Get(DistanceStrategy);
    }
}
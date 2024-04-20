namespace LangChain.Databases;

/// <summary>
/// Relevance score functions.
/// </summary>
public static class RelevanceScoreFunctions
{
    /// <summary>
    /// Return a similarity score on a scale [0, 1].
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float Euclidean(float distance)
    {
        // The 'correct' relevance function
        // may differ depending on a few things, including:
        // - the distance / similarity metric used by the VectorStore
        // - the scale of your embeddings (OpenAI's are unit normed. Many
        //  others are not!)
        // - embedding dimensionality
        // - etc.
        // This function converts the euclidean norm of normalized embeddings
        // (0 is most similar, sqrt(2) most dissimilar)
        // to a similarity function (0 to 1)
        return 1.0f - distance / (float)Math.Sqrt(2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float Cosine(float distance) => 1.0f - distance;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float MaxInnerProduct(float distance)
        => distance > 0
            ? 1.0f - distance
            : -1.0f * distance;

    public static Func<float, float> Get(DistanceStrategy distanceStrategy)
    {
        return distance =>
        {
            return distanceStrategy switch
            {
                DistanceStrategy.Euclidean => Euclidean(distance),
                DistanceStrategy.Cosine => Cosine(distance),
                DistanceStrategy.InnerProduct => MaxInnerProduct(distance),
                _ => throw new ArgumentOutOfRangeException(
                    $"No supported normalization function for {nameof(distanceStrategy)} of {distanceStrategy}." +
                    $"Consider providing nameof(overrideRelevanceScoreFn) to constructor.")
            };
        };
    }
}
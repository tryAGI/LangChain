using LangChain.Sources;

namespace LangChain.Databases;

/// <summary>
/// VectorStore. Check https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html
/// </summary>
public abstract class VectorStore(
    Func<float, float>? overrideRelevanceScoreFn = null)
{
    /// <summary>
    /// 
    /// </summary>
    protected Func<float, float>? OverrideRelevanceScoreFn { get; } = overrideRelevanceScoreFn;

    /// <summary>
    /// Run more texts through the embeddings and add to the vectorstore.
    /// </summary>
    /// <param name="texts">List of strings to add to the vectorstore.</param>
    /// <param name="metadatas">Optional list of metadatas associated with the texts.</param>
    /// <param name="embeddings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IEnumerable<string>> AddTextsAsync(
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IReadOnlyDictionary<string, object>>? metadatas = null,
        IReadOnlyCollection<float[]>? embeddings = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete by ids
    /// </summary>
    /// <param name="ids">List of Ids to delete</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a similarity search with scores.
    /// </summary>
    /// <param name="embedding">Embedding to look up documents similar to.</param>
    /// <param name="k">Number of Documents to return. Defaults to 4.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IEnumerable<(string Text, Dictionary<string, object>? Metadata, float Distance)>> SimilaritySearchByVectorWithScoreAsync(
        IEnumerable<float> embedding,
        int k = 4,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Return docs selected using the maximal marginal relevance.
    /// 
    /// Maximal marginal relevance optimizes for similarity to query AND diversity among selected documents.
    /// </summary>
    /// <param name="embedding">Embedding to look up documents similar to.</param>
    /// <param name="k">Number of Documents to return. Defaults to 4.</param>
    /// <param name="fetchK">Number of Documents to fetch to pass to MMR algorithm.</param>
    /// <param name="lambdaMult"> Number between 0 and 1 that determines the degree
    /// of diversity among the results with 0 corresponding
    /// to maximum diversity and 1 to minimum diversity.
    /// Defaults to 0.5.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Documents selected by maximal marginal relevance.</returns>
    public abstract Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(
        IEnumerable<float> embedding,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Return a similarity score on a scale [0, 1].
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    protected static float EuclideanRelevanceScoreFn(float distance)
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
    protected static float CosineRelevanceScoreFn(float distance) => 1.0f - distance;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    protected static float MaxInnerProductRelevanceScoreFn(float distance)
        => distance > 0
            ? 1.0f - distance
            : -1.0f * distance;

    /// <summary>
    /// The 'correct' relevance function
    /// may differ depending on a few things, including:
    /// - the distance / similarity metric used by the VectorStore
    /// - the scale of your embeddings (OpenAI's are unit normed. Many others are not!)
    /// - embedding dimensionality
    /// - etc.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public abstract Func<float, float> SelectRelevanceScoreFn();


}
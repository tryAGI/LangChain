using LangChain.Abstractions.Embeddings.Base;
using LangChain.Docstore;

namespace LangChain.VectorStores;

/// <summary>
/// VectorStore
/// <see cref="https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html"/>
/// </summary>
public abstract class VectorStore
{
    protected IEmbeddings Embeddings { get; }
    protected Func<float, float>? OverrideRelevanceScoreFn { get;  }

    protected VectorStore(IEmbeddings embeddings, Func<float, float>? overrideRelevanceScoreFn=null)
    {
        Embeddings = embeddings;
        OverrideRelevanceScoreFn = overrideRelevanceScoreFn;
    }


    /// <summary>
    /// Run more documents through the embeddings and add to the vectorstore.
    /// </summary>
    /// <param name="documents">Documents to add to the vectorstore.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Ids of the added texts.</returns>
    public abstract Task<IEnumerable<string>> AddDocumentsAsync(
        IEnumerable<Document> documents,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Run more texts through the embeddings and add to the vectorstore.
    /// </summary>
    /// <param name="texts">List of strings to add to the vectorstore.</param>
    /// <param name="metadatas">Optional list of metadatas associated with the texts.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IEnumerable<string>> AddTextsAsync(
        IEnumerable<string> texts,
        IEnumerable<Dictionary<string, object>>? metadatas = null,
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
    /// Return docs most similar to query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="k"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IEnumerable<Document>> SimilaritySearchAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Return docs most similar to embedding vector.
    /// </summary>
    /// <param name="embedding">Embedding to look up documents similar to.</param>
    /// <param name="k">Number of Documents to return. Defaults to 4.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(
        IEnumerable<float> embedding,
        int k = 4,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a similarity search with scores.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <param name="k">The number of results to return.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of tuples containing the document and its score.</returns>
    public abstract Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Default similarity search with relevance scores. Modify if necessary in subclass.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <param name="k">The number of results to return.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of tuples containing the document and its relevance score.</returns>
    public async Task<IEnumerable<(Document, float)>> SimilaritySearchWithRelevanceScoresCore(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var relevanceScoreFn = SelectRelevanceScoreFn();
        var docsAndScores = await SimilaritySearchWithScoreAsync(query, k, cancellationToken);

        return docsAndScores.Select(x => (x.Item1, relevanceScoreFn(x.Item2))).ToList();
    }

    /// <summary>
    /// Return docs and relevance scores in the range [0, 1].
    /// 0 is dissimilar, 1 is most similar.
    /// </summary>
    /// <param name="query">input text</param>
    /// <param name="k">Number of Documents to return. Defaults to 4.</param>
    /// <param name="scoreThreshold">a floating point value between 0 to 1 to filter the resulting set of retrieved docs</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<(Document, float)>> SimilaritySearchWithRelevanceScores(
        string query,
        int k = 4,
        float? scoreThreshold = null,
        CancellationToken cancellationToken = default)
    {
        var docsAndSimilarities = await SimilaritySearchWithRelevanceScoresCore(query, k, cancellationToken);
        var docsAndSimilaritiesArray = docsAndSimilarities as (Document, float)[] ?? docsAndSimilarities.ToArray();
        if (docsAndSimilaritiesArray.Any(x => x.Item2 < 0.0 || x.Item2 > 1.0))
        {
            throw new ArgumentException($"Relevance scores must be between 0 and 1, got {docsAndSimilarities}");
        }

        if (scoreThreshold == null)
        {
            return docsAndSimilaritiesArray;
        }

        var passedThreshold = docsAndSimilaritiesArray.Where(x => x.Item2 >= scoreThreshold).ToList();

        // TODO: log? if No relevant docs were retrieved using the relevance score threshold {scoreThreshold}
        
        return passedThreshold;
    }

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
    /// Return docs selected using the maximal marginal relevance.
    /// 
    /// Maximal marginal relevance optimizes for similarity to query AND diversity among selected documents.
    /// </summary>
    /// <param name="query">Query to look up documents similar to.</param>
    /// <param name="k">Number of Documents to return. Defaults to 4.</param>
    /// <param name="fetchK">Number of Documents to fetch to pass to MMR algorithm.</param>
    /// <param name="lambdaMult"> Number between 0 and 1 that determines the degree
    /// of diversity among the results with 0 corresponding
    /// to maximum diversity and 1 to minimum diversity.
    /// Defaults to 0.5.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Documents selected by maximal marginal relevance.</returns>
    public abstract Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(
        string query,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Return docs most similar to query using specified search type.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="searchType"></param>
    /// <param name="k">Number of Documents to return. Defaults to 4.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<IEnumerable<Document>> SearchAsync(
        string query,
        string searchType,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        return searchType switch
        {
            "similarity" => SimilaritySearchAsync(query, k, cancellationToken),
            "mmr" => MaxMarginalRelevanceSearch(query, k, cancellationToken: cancellationToken),
            _ => throw new ArgumentException(
                $"search_type of {searchType} not allowed. Expected search_type to be 'similarity' or 'mmr'.")
        };
    }

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

    protected static float CosineRelevanceScoreFn(float distance) => 1.0f - distance;

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
    protected abstract Func<float, float> SelectRelevanceScoreFn();

    public VectorStoreRetriever AsRetreiver(ESearchType searchType=ESearchType.Similarity, float? scoreThreshold = null)
    {
        return new VectorStoreRetriever(this, searchType, scoreThreshold);
    }
}
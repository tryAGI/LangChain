using LangChain.Providers;
using LangChain.Sources;

namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public static class VectorStoreExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string AsString(
        this IEnumerable<Document> documents,
        string separator = "\n\n")
    {
        return string.Join(separator, documents.Select(x => x.PageContent));
    }

    /// <summary>
    /// Return docs most similar to query.
    /// </summary>
    /// <param name="vectorStore"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="query"></param>
    /// <param name="k"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Document>> SimilaritySearchAsync(
        this VectorStore vectorStore,
        IEmbeddingModel embeddingModel,
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        var (embedding, _) = await embeddingModel.CreateEmbeddingsAsync(
            request: query,
            settings: null,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await vectorStore.SimilaritySearchByVectorAsync(embedding, k, cancellationToken).ConfigureAwait(false);
    }
    
    public static async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(
        this VectorStore vectorStore,
        IEmbeddingModel embeddingModel,
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        float[] embedding = await embeddingModel.CreateEmbeddingsAsync(
            query, null, cancellationToken).ConfigureAwait(false);

        var records = await vectorStore.SimilaritySearchByVectorWithScoreAsync(
                embedding, k, cancellationToken)
            .ConfigureAwait(false);
        
        return records.Select(r => (new Document(r.Text, r.Metadata), r.Distance));
    }

    public static async Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(
        this VectorStore vectorStore,
        IEmbeddingModel embeddingModel,
        string query,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        float[] embedding = await embeddingModel.CreateEmbeddingsAsync(query, null, cancellationToken).ConfigureAwait(false);

        return await vectorStore.MaxMarginalRelevanceSearchByVector(embedding, k, fetchK, lambdaMult, cancellationToken)
            .ConfigureAwait(false);
    }
    
    public static async Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(
        this VectorStore vectorStore,
        IEnumerable<float> embedding,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        
        var records = await vectorStore.SimilaritySearchByVectorWithScoreAsync(embedding, k, cancellationToken)
            .ConfigureAwait(false);

        return records.Select(r => new Document(r.Text, r.Metadata));
    }
    
    public static async Task<IEnumerable<string>> AddDocumentsAsync(
        this VectorStore vectorStore,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<Document> documents,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        return await vectorStore.AddTextsAsync(
            embeddingModel: embeddingModel,
            texts: documents.Select(x => x.PageContent).ToArray(), 
            metadatas: documents.Select(x => x.Metadata).ToArray(),
            cancellationToken).ConfigureAwait(false);
    }
    
    public static async Task<IEnumerable<string>> AddTextsAsync(
        this VectorStore vectorStore,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IReadOnlyDictionary<string, object>>? metadatas = null,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        float[][] embeddings = await embeddingModel
            .CreateEmbeddingsAsync(texts.ToArray(), null, cancellationToken)
            .ConfigureAwait(false);

        return await vectorStore.AddTextsAsync(
            texts: texts, 
            metadatas: metadatas,
            embeddings: embeddings,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Default similarity search with relevance scores. Modify if necessary in subclass.
    /// </summary>
    /// <param name="embeddingModel"></param>
    /// <param name="query">The query string.</param>
    /// <param name="k">The number of results to return.</param>
    /// <param name="cancellationToken"></param>
    /// <param name="vectorStore"></param>
    /// <returns>A list of tuples containing the document and its relevance score.</returns>
    public static async Task<IReadOnlyCollection<(Document, float)>> SimilaritySearchWithRelevanceScoresCore(
        this VectorStore vectorStore,
        IEmbeddingModel embeddingModel,
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        var relevanceScoreFn = vectorStore.SelectRelevanceScoreFn();
        var docsAndScores = await vectorStore.SimilaritySearchWithScoreAsync(embeddingModel, query, k, cancellationToken).ConfigureAwait(false);

        return docsAndScores.Select(x => (x.Item1, relevanceScoreFn(x.Item2))).ToList();
    }

    /// <summary>
    /// Return docs and relevance scores in the range [0, 1].
    /// 0 is dissimilar, 1 is most similar.
    /// </summary>
    /// <param name="vectorStore"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="query">input text</param>
    /// <param name="k">Number of Documents to return. Defaults to 4.</param>
    /// <param name="scoreThreshold">a floating point value between 0 to 1 to filter the resulting set of retrieved docs</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<(Document, float)>> SimilaritySearchWithRelevanceScores(
        this VectorStore vectorStore,
        IEmbeddingModel embeddingModel,
        string query,
        int k = 4,
        float? scoreThreshold = null,
        CancellationToken cancellationToken = default)
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));

        var docsAndSimilarities = await vectorStore.SimilaritySearchWithRelevanceScoresCore(embeddingModel, query, k, cancellationToken).ConfigureAwait(false);
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
}
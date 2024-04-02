using LangChain.Callback;
using LangChain.Providers;
using LangChain.Sources;
using LangChain.Retrievers;

namespace LangChain.Databases;

/// <summary>
/// Base Retriever class for VectorStore.
/// https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html
/// </summary>
public class VectorStoreRetriever : BaseRetriever
{
    /// <summary>
    /// 
    /// </summary>
    public VectorStore Vectorstore { get; init; }

    private VectorSearchType SearchType { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public int K { get; set; } = 4;

    private float? ScoreThreshold { get; init; }

    private IEmbeddingModel? EmbeddingModel { get; init; }

    /// <inheritdoc/>
    public VectorStoreRetriever(
        VectorStore vectorstore,
        VectorSearchType searchType = VectorSearchType.Similarity,
        float? scoreThreshold = null)
    {
        SearchType = searchType;

        if (SearchType == VectorSearchType.SimilarityScoreThreshold && ScoreThreshold == null)
            throw new ArgumentException($"ScoreThreshold required for {SearchType}");

        Vectorstore = vectorstore;
        SearchType = searchType;
        ScoreThreshold = scoreThreshold;
    }

    /// <inheritdoc/>
    protected override async Task<IEnumerable<Document>> GetRelevantDocumentsCoreAsync(
        string query,
        CallbackManagerForRetrieverRun? runManager = null)
    {
        switch (SearchType)
        {
            case VectorSearchType.Similarity:
                var embeddingModel = EmbeddingModel ?? throw new ArgumentException("EmbeddingModel required for Similarity search");
                return await Vectorstore.SimilaritySearchAsync(embeddingModel, query, K).ConfigureAwait(false);

            case VectorSearchType.SimilarityScoreThreshold:
                var docsAndSimilarities = await Vectorstore.SimilaritySearchWithRelevanceScores(query, K).ConfigureAwait(false);
                return docsAndSimilarities.Select(dws => dws.Item1);

            case VectorSearchType.MaximumMarginalRelevance:
                return await Vectorstore.MaxMarginalRelevanceSearch(query, K).ConfigureAwait(false);

            default:
                throw new ArgumentException($"{SearchType} not supported");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    public Task<IEnumerable<string>> AddDocumentsAsync(IEnumerable<Document> documents)
        => Vectorstore.AddDocumentsAsync(documents);
}
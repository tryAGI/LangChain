using LangChain.Callback;
using LangChain.Databases;
using LangChain.Providers;
using LangChain.Sources;

namespace LangChain.Retrievers;

/// <summary>
/// Base Retriever class for VectorStore.
/// https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html
/// </summary>
public class VectorStoreRetriever : BaseRetriever
{
    /// <summary>
    /// 
    /// </summary>
    public IVectorCollection VectorCollection { get; init; }

    private VectorSearchType SearchType { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public int K { get; set; } = 4;

    private float? ScoreThreshold { get; init; }

    private IEmbeddingModel EmbeddingModel { get; init; }

    /// <inheritdoc/>
    public VectorStoreRetriever(
        IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        VectorSearchType searchType = VectorSearchType.Similarity,
        float? scoreThreshold = null)
    {
        SearchType = searchType;

        if (SearchType == VectorSearchType.SimilarityScoreThreshold && ScoreThreshold == null)
            throw new ArgumentException($"ScoreThreshold required for {SearchType}");

        EmbeddingModel = embeddingModel;
        VectorCollection = vectorCollection;
        SearchType = searchType;
        ScoreThreshold = scoreThreshold;
    }

    /// <inheritdoc/>
    protected override async Task<IEnumerable<Document>> GetRelevantDocumentsCoreAsync(
        string query,
        CallbackManagerForRetrieverRun? runManager = null,
        CancellationToken cancellationToken = default)
    {
        var response = await VectorCollection.SearchAsync(EmbeddingModel, query, searchSettings: new VectorSearchSettings
        {
            Type = SearchType,
            NumberOfResults = K,
            ScoreThreshold = ScoreThreshold,
        }, cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.ToDocuments();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<string>> AddDocumentsAsync(IReadOnlyCollection<Document> documents)
    {
        return VectorCollection.AddDocumentsAsync(EmbeddingModel, documents);
    }
}
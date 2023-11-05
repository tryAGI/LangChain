using LangChain.Docstore;
using LangChain.Retrievers;

namespace LangChain.VectorStores;

/// <summary>
/// Base Retriever class for VectorStore.
/// <see cref="https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html" />
/// </summary>
public class VectorStoreRetriever : BaseRetriever
{
    public VectorStore Vectorstore { get; init; }


    private ESearchType SearchType { get; init; }
    private float? ScoreThreshold { get; init; }

    public VectorStoreRetriever(VectorStore vectorstore, ESearchType searchType = ESearchType.Similarity,
        float? scoreThreshold = null)
    {
        SearchType = searchType;

        if (SearchType == ESearchType.SimilarityScoreThreshold && ScoreThreshold == null)
            throw new ArgumentException($"ScoreThreshold required for {SearchType}");

        Vectorstore = vectorstore;
        SearchType = searchType;
        ScoreThreshold = scoreThreshold;
    }

    protected override async Task<IEnumerable<Document>> GetRelevantDocumentsAsync(string query, int k = 4)
    {
        switch (SearchType)
        {
            case ESearchType.Similarity:
                return await Vectorstore.SimilaritySearchAsync(query, k);

            case ESearchType.SimilarityScoreThreshold:
                var docsAndSimilarities = await Vectorstore.SimilaritySearchWithRelevanceScores(query, k);
                return docsAndSimilarities.Select(dws => dws.Item1);

            case ESearchType.MMR:
                return await Vectorstore.MaxMarginalRelevanceSearch(query, k);

            default:
                throw new ArgumentException($"{SearchType} not supported");
        }
    }

    public Task<IEnumerable<string>> AddDocumentsAsync(IEnumerable<Document> documents)
        => Vectorstore.AddDocumentsAsync(documents);
}
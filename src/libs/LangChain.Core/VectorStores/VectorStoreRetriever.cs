using LangChain.Docstore;
using LangChain.Retrievers;

namespace LangChain.VectorStores;

/// <summary>
/// Base Retriever class for VectorStore.
/// <see cref="https://api.python.langchain.com/en/latest/_modules/langchain/schema/vectorstore.html" />
/// </summary>
public class VectorStoreRetriever : BaseRetriever
{
    private static readonly HashSet<string> _allowedSearchTypes = new() { "similarity", "similarity_score_threshold", "mmr" };

    public VectorStore Vectorstore { get; init; }
    
    // TODO: enum
    private string SearchType { get; init; } = "similarity";
    private float? ScoreThreshold { get; init; }

    public VectorStoreRetriever(VectorStore vectorstore, string searchType, float? scoreThreshold)
    {
        if (!_allowedSearchTypes.Contains(searchType))
            throw new ArgumentException($"{searchType} not supported");

        if (SearchType == "similarity_score_threshold" && ScoreThreshold == null)
            throw new ArgumentException($"ScoreThreshold required for {SearchType}");

        Vectorstore = vectorstore;
        SearchType = searchType;
        ScoreThreshold = scoreThreshold;
    }

    protected override async Task<IEnumerable<Document>> GetRelevantDocumentsAsync(string query, int k = 4)
    {
        switch (SearchType)
        {
            case "similarity":
                return await Vectorstore.SimilaritySearchAsync(query, k);

            case "similarity_score_threshold":
                var docsAndSimilarities = await Vectorstore.SimilaritySearchWithRelevanceScores(query, k);
                return docsAndSimilarities.Select(dws => dws.Item1);

            case "mmr":
                return await Vectorstore.MaxMarginalRelevanceSearch(query, k);

            default:
                throw new ArgumentException($"{SearchType} not supported");
        }
    }

    public Task<IEnumerable<string>> AddDocumentsAsync(IEnumerable<Document> documents)
        => Vectorstore.AddDocumentsAsync(documents);
}
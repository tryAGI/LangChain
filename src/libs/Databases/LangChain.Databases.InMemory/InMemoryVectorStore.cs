using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.Providers;
using LangChain.TextSplitters;
using LangChain.VectorStores;

namespace LangChain.Databases.InMemory;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="embeddings"></param>
/// <param name="distanceMetrics"></param>
public class InMemoryVectorStore(
    IEmbeddingModel embeddings,
    EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean)
    : VectorStore(embeddings)
{
    private readonly Func<float[], float[], float> _distanceFunction =
        distanceMetrics == EDistanceMetrics.Euclidean
            ? Utils.ComputeEuclideanDistance
            : Utils.ComputeManhattanDistance;

    private readonly List<(float[] vec, string id, Document doc)> _storage = [];
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="embeddings"></param>
    /// <param name="documents"></param>
    /// <returns></returns>
    public static async Task<VectorStoreIndexWrapper> CreateIndexFromDocuments(
        IEmbeddingModel embeddings,
        IReadOnlyCollection<Document> documents)
    {
        var vectorStore = new InMemoryVectorStore(embeddings);
        var textSplitter = new CharacterTextSplitter();
        var indexCreator = new VectorStoreIndexCreator(vectorStore, textSplitter);
        var index = await indexCreator.FromDocumentsAsync(documents).ConfigureAwait(false);
        
        return index;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddDocumentsAsync(
        IEnumerable<Document> documents,
        CancellationToken cancellationToken = default)
    {
        var docs = documents.ToArray();

        float[][] embeddings = await EmbeddingModel.CreateEmbeddingsAsync(docs
            .Select(x => x.PageContent)
            .ToArray(), cancellationToken: cancellationToken).ConfigureAwait(false);
        var ids = new List<string>();
        for (var i = 0; i < docs.Length; i++)
        {
            var id = Guid.NewGuid().ToString();
            ids.Add(id);
            _storage.Add((embeddings[i], id, docs[i]));
        }

        return ids;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<string>> AddTextsAsync(
        IEnumerable<string> texts,
        IEnumerable<Dictionary<string, object>>? metadatas = null,
        CancellationToken cancellationToken = default)
    {
        var docs = metadatas != null 
            ? texts
                .Zip(metadatas, (d, m) => new Document(d, m))
                .ToArray()
            : texts
                .Select(d => new Document(d))
                .ToArray();

        return await AddDocumentsAsync(docs, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        _storage.RemoveAll(s => ids.Contains(s.id));
        
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<Document>> SimilaritySearchAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        float[] embedding = await EmbeddingModel.CreateEmbeddingsAsync(
            query,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await SimilaritySearchByVectorAsync(
            embedding,
            k,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(
        IEnumerable<float> embedding,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var arr = embedding.ToArray();
        var distances = _storage
            .OrderBy(s => _distanceFunction(s.vec, arr))
            .Take(k)
            .Select(d => d.doc);
        
        return Task.FromResult(distances);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(
        string query,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        float[] embedding = await EmbeddingModel.CreateEmbeddingsAsync(
            query,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        var distances = _storage.Select(s =>
            new
            {
                doc = s.doc,
                distance = _distanceFunction(s.vec, embedding)
            }).Take(k);
        
        return distances.Select(d => new ValueTuple<Document, float>(d.doc, d.distance));
    }

    /// <inheritdoc />
    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(IEnumerable<float> embedding, int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        return Task.FromException<IEnumerable<Document>>(new NotImplementedException());
    }

    /// <inheritdoc />
    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(
        string query,
        int k = 4,
        int fetchK = 20,
        float lambdaMult = 0.5f,
        CancellationToken cancellationToken = default)
    {
        return Task.FromException<IEnumerable<Document>>(new NotImplementedException());
    }

    /// <inheritdoc />
    protected override Func<float, float> SelectRelevanceScoreFn()
    {
        throw new NotImplementedException();
    }
}
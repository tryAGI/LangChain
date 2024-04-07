using LangChain.Providers;
using LangChain.Sources;

namespace LangChain.Databases;

/// <summary>
/// 
/// </summary>
public static class VectorDatabaseExtensions
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
    /// <param name="vectorDatabase"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="embeddingRequest"></param>
    /// <param name="embeddingSettings"></param>
    /// <param name="searchSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<VectorSearchResponse> SearchAsync(
        this IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        EmbeddingRequest embeddingRequest,
        EmbeddingSettings? embeddingSettings = default,
        VectorSearchSettings? searchSettings = default,
        CancellationToken cancellationToken = default)
    {
        vectorDatabase = vectorDatabase ?? throw new ArgumentNullException(nameof(vectorDatabase));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        var response = await embeddingModel.CreateEmbeddingsAsync(
            request: embeddingRequest,
            settings: embeddingSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await vectorDatabase.SearchAsync(new VectorSearchRequest
        {
            Embeddings = [response.ToSingleArray()],
        }, searchSettings, cancellationToken).ConfigureAwait(false);
    }
    
    public static async Task<IEnumerable<string>> AddDocumentsAsync(
        this IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<Document> documents,
        CancellationToken cancellationToken = default)
    {
        vectorDatabase = vectorDatabase ?? throw new ArgumentNullException(nameof(vectorDatabase));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        return await vectorDatabase.AddTextsAsync(
            embeddingModel: embeddingModel,
            texts: documents.Select(x => x.PageContent).ToArray(), 
            metadatas: documents.Select(x => x.Metadata).ToArray(),
            cancellationToken).ConfigureAwait(false);
    }
    
    public static async Task<IEnumerable<string>> AddTextsAsync(
        this IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IReadOnlyDictionary<string, object>>? metadatas = null,
        CancellationToken cancellationToken = default)
    {
        vectorDatabase = vectorDatabase ?? throw new ArgumentNullException(nameof(vectorDatabase));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        float[][] embeddings = await embeddingModel
            .CreateEmbeddingsAsync(texts.ToArray(), null, cancellationToken)
            .ConfigureAwait(false);

        return await vectorDatabase.AddAsync(
            items: texts.Select((text, i) => new VectorSearchItem
            {
                Text = text,
                Metadata = metadatas?.ElementAt(i),
                Embedding = embeddings[i],
            }).ToArray(),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Default similarity search with relevance scores. Modify if necessary in subclass.
    /// Return docs and relevance scores in the range [0, 1].
    /// 0 is dissimilar, 1 is most similar.
    /// </summary>
    /// <param name="embeddingModel"></param>
    /// <param name="embeddingRequest">The query string(string will be converted implicitly) or embedding request.</param>
    /// <param name="searchSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="vectorDatabase"></param>
    /// <param name="embeddingSettings"></param>
    /// <returns>A list of tuples containing the document and its relevance score.</returns>
    public static async Task<VectorSearchResponse> SearchWithRelevanceScoresAsync(
        this IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        EmbeddingRequest embeddingRequest,
        EmbeddingSettings? embeddingSettings = default,
        VectorSearchSettings? searchSettings = default,
        CancellationToken cancellationToken = default)
    {
        vectorDatabase = vectorDatabase ?? throw new ArgumentNullException(nameof(vectorDatabase));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        
        var response = await vectorDatabase.SearchAsync(
            embeddingModel: embeddingModel,
            embeddingRequest: embeddingRequest,
            embeddingSettings: embeddingSettings,
            searchSettings: searchSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        var relevanceScoreFunc = searchSettings?.RelevanceScoreFunc ?? RelevanceScoreFunctions.Euclidean;
        foreach (var item in response.Items)
        {
            item.RelevanceScore = Math.Max(0.0F, Math.Min(1.0F, relevanceScoreFunc(item.Distance)));
        }

        if (searchSettings?.ScoreThreshold == null)
        {
            return response;
        }

        return new VectorSearchResponse
        {
            Items = response.Items
                .Where(x => x.RelevanceScore >= searchSettings.ScoreThreshold)
                .ToArray(),
        };
    }
}
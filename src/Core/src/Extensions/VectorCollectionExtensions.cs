using LangChain.Databases;
using LangChain.Providers;
using LangChain.DocumentLoaders;

namespace LangChain.Extensions;

/// <summary>
/// 
/// </summary>
public static class VectorCollectionExtensions
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
    /// Return documents most similar to query.
    /// </summary>
    /// <param name="vectorCollection"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="embeddingRequest"></param>
    /// <param name="embeddingSettings"></param>
    /// <param name="searchSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<VectorSearchResponse> SearchAsync(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        EmbeddingRequest embeddingRequest,
        EmbeddingSettings? embeddingSettings = default,
        VectorSearchSettings? searchSettings = default,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));
        searchSettings ??= new VectorSearchSettings();

        if (searchSettings is { Type: VectorSearchType.SimilarityScoreThreshold, ScoreThreshold: null })
        {
            throw new ArgumentException($"ScoreThreshold required for {searchSettings.Type}");
        }

        var response = await embeddingModel.CreateEmbeddingsAsync(
            request: embeddingRequest,
            settings: embeddingSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await vectorCollection.SearchAsync(new VectorSearchRequest
        {
            Embeddings = [response.ToSingleArray()],
        }, searchSettings, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Return similar documents to the given document.
    /// </summary>
    /// <param name="vectorCollection">vector store</param>
    /// <param name="embeddingModel"></param>
    /// <param name="request"></param>
    /// <param name="amount"></param>
    /// <param name="searchType">search type</param>
    /// <param name="scoreThreshold">score threshold</param>
    /// <param name="embeddingSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyCollection<Document>> GetSimilarDocuments(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        EmbeddingRequest request,
        int amount = 4,
        VectorSearchType searchType = VectorSearchType.Similarity,
        float? scoreThreshold = null,
        EmbeddingSettings? embeddingSettings = null,
        CancellationToken cancellationToken = default)
    {
        var results = await vectorCollection.SearchAsync(
            embeddingModel: embeddingModel,
            embeddingRequest: request,
            embeddingSettings: embeddingSettings,
            searchSettings: new VectorSearchSettings
            {
                Type = searchType,
                NumberOfResults = amount,
                ScoreThreshold = scoreThreshold
            },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return results.ToDocuments();
    }

    public static async Task<IReadOnlyCollection<string>> AddDocumentsAsync(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<Document> documents,
        EmbeddingSettings? embeddingSettings = default,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));

        return await vectorCollection.AddTextsAsync(
            embeddingModel: embeddingModel,
            texts: documents.Select(x => x.PageContent).ToArray(),
            metadatas: documents.Select(x => x.Metadata).ToArray(),
            embeddingSettings: embeddingSettings,
            cancellationToken).ConfigureAwait(false);
    }

    public static async Task<Document?> GetDocumentByIdAsync(
        this IVectorCollection vectorCollection,
        string id,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));

        var item = await vectorCollection.GetAsync(id, cancellationToken).ConfigureAwait(false);

        return item == null
            ? null
            : new Document(item.Text, item.Metadata);
    }

    public static async Task<IReadOnlyCollection<string>> AddTextsAsync(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IReadOnlyDictionary<string, object>>? metadatas = null,
        EmbeddingSettings? embeddingSettings = default,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));

        var embeddingRequest = new EmbeddingRequest
        {
            Strings = texts.ToArray(),
            Images = metadatas?
                .Select((metadata, i) => metadata.TryGetValue(texts.ElementAt(i), out object? result)
                    ? result as BinaryData
                    : null)
                .Where(x => x != null)
                .Select(x => Data.FromBytes(x!.ToArray()))
                .ToArray() ?? [],
        };

        float[][] embeddings = await embeddingModel
            .CreateEmbeddingsAsync(embeddingRequest, embeddingSettings, cancellationToken)
            .ConfigureAwait(false);

        return await vectorCollection.AddAsync(
            items: texts.Select((text, i) => new Vector
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
    /// <param name="vectorCollection"></param>
    /// <param name="embeddingSettings"></param>
    /// <returns>A list of tuples containing the document and its relevance score.</returns>
    public static async Task<VectorSearchResponse> SearchWithRelevanceScoresAsync(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        EmbeddingRequest embeddingRequest,
        EmbeddingSettings? embeddingSettings = default,
        VectorSearchSettings? searchSettings = default,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingModel = embeddingModel ?? throw new ArgumentNullException(nameof(embeddingModel));

        var response = await vectorCollection.SearchAsync(
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
using System.Text.Json;
using LangChain.DocumentLoaders;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LangChain.Extensions;

/// <summary>
/// Extension methods for <see cref="VectorStoreCollection{TKey, TRecord}"/> with <see cref="LangChainDocumentRecord"/>.
/// </summary>
public static class VectorCollectionExtensions
{
    /// <summary>
    /// Converts a collection of documents to a single string.
    /// </summary>
    public static string AsString(
        this IEnumerable<Document> documents,
        string separator = "\n\n")
    {
        return string.Join(separator, documents.Select(x => x.PageContent));
    }

    /// <summary>
    /// Return documents most similar to query.
    /// </summary>
    public static async Task<IReadOnlyCollection<Document>> GetSimilarDocuments(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string query,
        int amount = 4,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

        var result = await embeddingGenerator.GenerateAsync(
            [query],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var embedding = result[0].Vector;

        var searchResults = vectorCollection.SearchAsync(
            embedding,
            amount,
            cancellationToken: cancellationToken);

        var documents = new List<Document>();
        await foreach (var searchResult in searchResults.ConfigureAwait(false))
        {
            documents.Add(RecordToDocument(searchResult.Record));
        }

        return documents;
    }

    /// <summary>
    /// Returns documents most similar to a query from a dynamically mapped collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<Document>> GetSimilarDocuments(
        this VectorStoreCollection<object, Dictionary<string, object?>> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string query,
        int amount = 4,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

        var result = await embeddingGenerator.GenerateAsync(
            [query],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var searchResults = vectorCollection.SearchAsync(
            result[0].Vector,
            amount,
            cancellationToken: cancellationToken);

        var documents = new List<Document>();
        await foreach (var searchResult in searchResults.ConfigureAwait(false))
        {
            documents.Add(LangChainDocumentRecordMapper.RecordToDocument(searchResult.Record));
        }

        return documents;
    }

    /// <summary>
    /// Add documents to the vector collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddDocumentsAsync(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IReadOnlyCollection<Document> documents,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

        return await vectorCollection.AddTextsAsync(
            embeddingGenerator: embeddingGenerator,
            texts: documents.Select(x => x.PageContent).ToArray(),
            metadatas: documents.Select(x => x.Metadata).ToArray(),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds documents to a dynamically mapped vector collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddDocumentsAsync(
        this VectorStoreCollection<object, Dictionary<string, object?>> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IReadOnlyCollection<Document> documents,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

        return await vectorCollection.AddTextsAsync(
            embeddingGenerator: embeddingGenerator,
            texts: documents.Select(x => x.PageContent).ToArray(),
            metadatas: documents.Select(x => x.Metadata).ToArray(),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a document by its ID.
    /// </summary>
    public static async Task<Document?> GetDocumentByIdAsync(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        string id,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));

        var item = await vectorCollection.GetAsync(id, cancellationToken: cancellationToken).ConfigureAwait(false);

        return item == null
            ? null
            : RecordToDocument(item);
    }

    /// <summary>
    /// Gets a document by its ID from a dynamically mapped vector collection.
    /// </summary>
    public static async Task<Document?> GetDocumentByIdAsync(
        this VectorStoreCollection<object, Dictionary<string, object?>> vectorCollection,
        string id,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));

        var item = await vectorCollection.GetAsync(id, cancellationToken: cancellationToken).ConfigureAwait(false);

        return item == null
            ? null
            : LangChainDocumentRecordMapper.RecordToDocument(item);
    }

    /// <summary>
    /// Add texts with optional metadata to the vector collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddTextsAsync(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IDictionary<string, object>>? metadatas = null,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

        var records = await CreateRecordsAsync(
            embeddingGenerator,
            texts,
            metadatas,
            cancellationToken).ConfigureAwait(false);

        foreach (var record in records)
        {
            await vectorCollection.UpsertAsync(record, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return records.Select(record => record.Id).ToArray();
    }

    /// <summary>
    /// Adds texts with optional metadata to a dynamically mapped vector collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddTextsAsync(
        this VectorStoreCollection<object, Dictionary<string, object?>> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IDictionary<string, object>>? metadatas = null,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        texts = texts ?? throw new ArgumentNullException(nameof(texts));

        var records = await CreateRecordsAsync(
            embeddingGenerator,
            texts,
            metadatas,
            cancellationToken).ConfigureAwait(false);

        foreach (var record in records)
        {
            await vectorCollection.UpsertAsync(
                LangChainDocumentRecordMapper.ToDynamicRecord(record),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return records.Select(record => record.Id).ToArray();
    }

    private static Document RecordToDocument(LangChainDocumentRecord record)
    {
        return LangChainDocumentRecordMapper.RecordToDocument(record);
    }

    private static async Task<List<LangChainDocumentRecord>> CreateRecordsAsync(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IReadOnlyCollection<string> texts,
        IReadOnlyCollection<IDictionary<string, object>>? metadatas,
        CancellationToken cancellationToken)
    {
        var result = await embeddingGenerator
            .GenerateAsync(texts.ToList(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var records = new List<LangChainDocumentRecord>(texts.Count);
        var index = 0;
        foreach (var text in texts)
        {
            records.Add(new LangChainDocumentRecord
            {
                Text = text,
                MetadataJson = metadatas?.ElementAt(index) is { } meta
                    ? JsonSerializer.Serialize(meta, LangChainJsonSerializerContext.Metadata.IDictionaryStringObject)
                    : null,
                Embedding = result[index].Vector,
            });

            index++;
        }

        return records;
    }
}

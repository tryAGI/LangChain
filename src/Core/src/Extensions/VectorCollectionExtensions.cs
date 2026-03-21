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

        var result = await embeddingGenerator
            .GenerateAsync(texts.ToList(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var ids = new List<string>();
        var index = 0;
        foreach (var text in texts)
        {
            var record = new LangChainDocumentRecord
            {
                Text = text,
                MetadataJson = metadatas?.ElementAt(index) is { } meta
                    ? JsonSerializer.Serialize(meta)
                    : null,
                Embedding = result[index].Vector,
            };

            await vectorCollection.UpsertAsync(record, cancellationToken: cancellationToken).ConfigureAwait(false);
            ids.Add(record.Id);
            index++;
        }

        return ids;
    }

    private static Document RecordToDocument(LangChainDocumentRecord record)
    {
        Dictionary<string, object>? metadata = null;
        if (record.MetadataJson is { Length: > 0 })
        {
            metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(record.MetadataJson);
        }

        return new Document(record.Text ?? string.Empty, metadata);
    }
}

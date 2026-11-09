using System.Text.Json;
using LangChain.DocumentLoaders;
using LangChain.Schema;
using LangChain.Splitters.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LangChain.Extensions;

/// <summary>
/// Source-scoped replacement helpers for callers that persist the record IDs returned at ingestion.
/// No provider-specific metadata query or collection schema migration is required.
/// </summary>
public static class SourceDocumentReplacementExtensions
{
    /// <summary>
    /// Replaces a known source's records in a typed collection, preserving other sources.
    /// Store the returned IDs with the source identifier for future updates.
    /// </summary>
    /// <remarks>
    /// Source metadata must contain an exact string "path" (or "source" when no "path" exists). Existing IDs are checked before
    /// changes are made. New documents are inserted before existing records are removed.
    /// Vector stores do not provide a cross-provider transaction: a write/delete failure can
    /// leave both old and new records or a partially updated source.
    /// </remarks>
    public static async Task<IReadOnlyCollection<string>> ReplaceSourceDocumentsAsync(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string source,
        IReadOnlyCollection<string> previousDocumentIds,
        IReadOnlyCollection<Document> documents,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

        var ids = ValidateArguments(source, previousDocumentIds, documents);
        foreach (var id in ids)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var existing = await vectorCollection.GetDocumentByIdAsync(id, cancellationToken).ConfigureAwait(false);
            EnsureSource(existing, source, id);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var replacementIds = await vectorCollection.AddDocumentsAsync(
            embeddingGenerator, documents, cancellationToken).ConfigureAwait(false);

        foreach (var id in ids)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await vectorCollection.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        }

        return replacementIds;
    }

    /// <summary>
    /// Replaces a known source's records in a dynamically mapped collection.
    /// Store the returned IDs with the source identifier for future updates.
    /// </summary>
    /// <remarks>
    /// Does not enumerate records or search JSON metadata. Requires saved record IDs
    /// from a previous ingestion; see the typed overload for failure semantics.
    /// </remarks>
    public static async Task<IReadOnlyCollection<string>> ReplaceSourceDocumentsAsync(
        this VectorStoreCollection<object, Dictionary<string, object?>> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string source,
        IReadOnlyCollection<string> previousDocumentIds,
        IReadOnlyCollection<Document> documents,
        CancellationToken cancellationToken = default)
    {
        vectorCollection = vectorCollection ?? throw new ArgumentNullException(nameof(vectorCollection));
        embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));

        var ids = ValidateArguments(source, previousDocumentIds, documents);
        foreach (var id in ids)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var existing = await vectorCollection.GetDocumentByIdAsync(id, cancellationToken).ConfigureAwait(false);
            EnsureSource(existing, source, id);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var replacementIds = await vectorCollection.AddDocumentsAsync(
            embeddingGenerator, documents, cancellationToken).ConfigureAwait(false);

        foreach (var id in ids)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await vectorCollection.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        }

        return replacementIds;
    }

    /// <summary>
    /// Reloads, splits, and replaces a known source's records in a typed collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> ReplaceSourceDocumentsFromAsync<TLoader>(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string source,
        IReadOnlyCollection<string> previousDocumentIds,
        DataSource dataSource,
        ITextSplitter? textSplitter = null,
        DocumentLoaderSettings? loaderSettings = null,
        CancellationToken cancellationToken = default)
        where TLoader : IDocumentLoader, new()
    {
        dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        loaderSettings ??= new DocumentLoaderSettings();
        var documents = await new TLoader().LoadAsync(
            dataSource: dataSource,
            settings: loaderSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        textSplitter ??= new CharacterTextSplitter();
        return await vectorCollection.ReplaceSourceDocumentsAsync(
            embeddingGenerator, source, previousDocumentIds,
            textSplitter.SplitDocuments(documents), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reloads, splits, and replaces a known source's records in a dynamically mapped collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> ReplaceSourceDocumentsFromAsync<TLoader>(
        this VectorStoreCollection<object, Dictionary<string, object?>> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        string source,
        IReadOnlyCollection<string> previousDocumentIds,
        DataSource dataSource,
        ITextSplitter? textSplitter = null,
        DocumentLoaderSettings? loaderSettings = null,
        CancellationToken cancellationToken = default)
        where TLoader : IDocumentLoader, new()
    {
        dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        loaderSettings ??= new DocumentLoaderSettings();
        var documents = await new TLoader().LoadAsync(
            dataSource: dataSource,
            settings: loaderSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        textSplitter ??= new CharacterTextSplitter();
        return await vectorCollection.ReplaceSourceDocumentsAsync(
            embeddingGenerator, source, previousDocumentIds,
            textSplitter.SplitDocuments(documents), cancellationToken).ConfigureAwait(false);
    }

    private static IReadOnlyCollection<string> ValidateArguments(
        string source,
        IReadOnlyCollection<string> previousDocumentIds,
        IReadOnlyCollection<Document> documents)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("A non-empty source identifier is required.", nameof(source));
        }

        previousDocumentIds = previousDocumentIds ??
            throw new ArgumentNullException(nameof(previousDocumentIds));
        documents = documents ?? throw new ArgumentNullException(nameof(documents));

        if (documents.Count == 0)
        {
            throw new ArgumentException("Replacement documents cannot be empty.", nameof(documents));
        }

        foreach (var document in documents)
        {
            EnsureSource(document, source);
        }

        var ids = previousDocumentIds.Distinct(StringComparer.Ordinal).ToArray();
        if (ids.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Previous record IDs cannot be null or blank.", nameof(previousDocumentIds));
        }

        return ids;
    }

    private static void EnsureSource(Document? document, string expectedSource, string? id = null)
    {
        if (document is null ||
            (!document.Metadata.TryGetValue("path", out var value) &&
             !document.Metadata.TryGetValue("source", out value)) ||
            !MatchesSource(value, expectedSource))
        {
            var suffix = id is null ? string.Empty : $" (record ID: {id})";
            throw new InvalidOperationException(
                $"Document path/source metadata must match '{expectedSource}'{suffix}.");
        }
    }

    private static bool MatchesSource(object? value, string source)
    {
        return value switch
        {
            string text => string.Equals(text, source, StringComparison.Ordinal),
            JsonElement { ValueKind: JsonValueKind.String } element =>
                string.Equals(element.GetString(), source, StringComparison.Ordinal),
            _ => false,
        };
    }
}

using LangChain.Sources;
using LangChain.Splitters.Text;
using LangChain.Databases;
using LangChain.Extensions;
using LangChain.Providers;

namespace LangChain.Indexes;

/// <summary>
/// Logic for creating a VectorDatabases tables.
/// </summary>
/// // embeddings are not needed here because VectorStore already has them
public static class VectorStoreIndexCreator
{
    /// <summary>
    /// Create a VectorDatabase table from loaders.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> LoadAndSplitDocuments(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<ISource> sources,
        ITextSplitter? textSplitter = null,
        CancellationToken cancellationToken = default)
    {
        sources = sources ?? throw new ArgumentNullException(nameof(sources));

        var documents = new List<Document>();
        foreach (var source in sources)
        {
            documents.AddRange(await source.LoadAsync(cancellationToken).ConfigureAwait(false));
        }

        return await vectorCollection.AddSplitDocumentsAsync(embeddingModel, documents, textSplitter).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a VectorDatabase table from documents.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddSplitDocumentsAsync(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<Document> documents,
        ITextSplitter? textSplitter = null)
    {
        textSplitter ??= new CharacterTextSplitter();

        var splitDocuments = textSplitter.SplitDocuments(documents);

        return await vectorCollection.AddDocumentsAsync(embeddingModel, splitDocuments).ConfigureAwait(false);
    }
}
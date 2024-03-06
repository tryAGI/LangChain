using LangChain.Base;
using LangChain.Extensions;
using LangChain.Sources;
using LangChain.Splitters.Text;
using LangChain.VectorStores;

namespace LangChain.Indexes;

/// <summary>
/// Logic for creating a vectorstore index.
/// </summary>
/// // embeddings are not needed here because VectorStore already has them
public class VectorStoreIndexCreator(
    VectorStore vectorStore,
    TextSplitter textSplitter)
{
    /// <summary>
    /// 
    /// </summary>
    public VectorStore VectorStore { get; } = vectorStore;

    /// <summary>
    /// 
    /// </summary>
    public TextSplitter TextSplitter { get; } = textSplitter;

    /// <summary>
    /// Create a vectorstore index from loaders.
    /// </summary>
    public async Task<VectorStoreIndexWrapper> FromLoaders(List<FileSource> loaders, CancellationToken cancellationToken = default)
    {
        loaders = loaders ?? throw new ArgumentNullException(nameof(loaders));
        
        List<Document> documents = new();
        foreach (var loader in loaders)
        {
            documents.AddRange(await loader.LoadAsync(cancellationToken).ConfigureAwait(false));
        }

        return await FromDocumentsAsync(documents).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a vectorstore index from documents.
    /// </summary>
    public async Task<VectorStoreIndexWrapper> FromDocumentsAsync(IReadOnlyCollection<Document> documents)
    {
        var subDocs = TextSplitter.SplitDocuments(documents);
        
        await VectorStore.AddDocumentsAsync(subDocs).ConfigureAwait(false);
        
        return new VectorStoreIndexWrapper(VectorStore);
    }
}
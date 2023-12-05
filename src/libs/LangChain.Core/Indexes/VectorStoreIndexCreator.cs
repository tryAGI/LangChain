using LangChain.Abstractions.Embeddings.Base;
using LangChain.Base;
using LangChain.Docstore;
using LangChain.VectorStores;

namespace LangChain.Indexes;

/// <summary>
/// Logic for creating a vectorstore index.
/// </summary>
public class VectorStoreIndexCreator
{
    public VectorStore VectorStore { get; }
    public TextSplitter TextSplitter { get; }

    // embeddings are not needed here because VectorStore already has them
    public VectorStoreIndexCreator(VectorStore vectorStore, TextSplitter textSplitter)
    {
        VectorStore = vectorStore;
        TextSplitter = textSplitter;
    }

    /// <summary>
    /// Create a vectorstore index from loaders.
    /// </summary>
    public async Task<VectorStoreIndexWrapper> FromLoaders(List<BaseLoader> loaders)
    {
        loaders = loaders ?? throw new ArgumentNullException(nameof(loaders));
        
        List<Document> documents = new();
        foreach (var loader in loaders)
        {
            documents.AddRange(loader.Load());
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
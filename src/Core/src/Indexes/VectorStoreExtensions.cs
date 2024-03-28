using LangChain.Sources;
using LangChain.Splitters.Text;
using LangChain.VectorStores;

namespace LangChain.Indexes;

public static class VectorStoreExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vectorStore"></param>
    /// <param name="documents"></param>
    /// <param name="textSplitter"></param>
    /// <returns></returns>
    public static async Task<VectorStoreIndexWrapper> CreateIndexFromDocuments(
        this VectorStore vectorStore,
        IReadOnlyCollection<Document> documents,
        ITextSplitter? textSplitter = null) 
    {
        textSplitter ??= new CharacterTextSplitter();
        var indexCreator = new VectorStoreIndexCreator(vectorStore, textSplitter);
        var index = await indexCreator.FromDocumentsAsync(documents).ConfigureAwait(false);
        
        return index;
    }

    /// <summary>
    ///  If database does not exists, it loads documents from the documentsSource, creates an index from these documents and returns the created index.
    ///  If database exists, it loads the index from the database.
    ///  documentsSource is used only if the database does not exist. If the database exists, documentsSource is ignored.
    /// </summary>
    /// <param name="vectorStore">An object implementing the <see cref="VectorStore"/> interface. This object is used to generate embeddings for the documents.</param>
    /// <param name="documentsSource">An optional object implementing the <see cref="ISource"/> interface. This object is used to load documents if the vector store database file does not exist.</param>
    /// <param name="textSplitter"></param>
    /// <param name="cancellationToken"></param>
    public static async Task<VectorStoreIndexWrapper> GetOrCreateIndexAsync(
        this VectorStore vectorStore,
        ISource? documentsSource = null,
        ITextSplitter? textSplitter = null,
        CancellationToken cancellationToken = default)
    {
        if (documentsSource != null)
        {
            var documents = await documentsSource.LoadAsync(cancellationToken).ConfigureAwait(false);
            
            return await vectorStore.CreateIndexFromDocuments(
                documents,
                textSplitter: textSplitter).ConfigureAwait(false);
        }

        var index = new VectorStoreIndexWrapper(vectorStore);
        return index;
    }
}
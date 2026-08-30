using LangChain.DocumentLoaders;
using LangChain.Schema;
using LangChain.Splitters.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace LangChain.Extensions;

public static class VectorDatabaseExtensions
{
    /// <summary>
    /// Adds documents loaded using <typeparamref name="TLoader"/> to the vector store. <br/>
    /// If the collection does not exist, it will be created. <br/>
    /// Splitting the text into sentences is done using the default text splitter, but you can provide your own. <br/>
    /// </summary>
    public static async Task<VectorStoreCollection<string, LangChainDocumentRecord>> AddDocumentsFromAsync<TLoader>(
        this VectorStore vectorStore,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        int dimensions,
        DataSource dataSource,
        string collectionName = "langchain",
        ITextSplitter? textSplitter = null,
        DocumentLoaderSettings? loaderSettings = null,
        AddDocumentsToDatabaseBehavior behavior = AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists,
        CancellationToken cancellationToken = default)
        where TLoader : IDocumentLoader, new()
    {
        vectorStore = vectorStore ?? throw new ArgumentNullException(paramName: nameof(vectorStore));

        var definition = new VectorStoreCollectionDefinition
        {
            Properties =
            [
                new VectorStoreKeyProperty("Id", typeof(string)),
                new VectorStoreDataProperty("Text", typeof(string)),
                new VectorStoreDataProperty("MetadataJson", typeof(string)),
                new VectorStoreVectorProperty("Embedding", dimensions) { Type = typeof(ReadOnlyMemory<float>) },
            ]
        };

        var collection = vectorStore.GetCollection<string, LangChainDocumentRecord>(collectionName, definition);

        if (behavior == AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists &&
            await collection.CollectionExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return collection;
        }

        if (behavior == AddDocumentsToDatabaseBehavior.OverwriteExistingCollection &&
            await collection.CollectionExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            await collection.EnsureCollectionDeletedAsync(cancellationToken).ConfigureAwait(false);
        }

        await collection.EnsureCollectionExistsAsync(cancellationToken).ConfigureAwait(false);

        await collection.AddDocumentsFromAsync<TLoader>(
            embeddingGenerator: embeddingGenerator,
            dataSource: dataSource,
            textSplitter: textSplitter,
            loaderSettings: loaderSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return collection;
    }

    /// <summary>
    /// Adds documents loaded using <typeparamref name="TLoader"/> to the vector collection. <br/>
    /// Splitting the text into sentences is done using the default text splitter, but you can provide your own. <br/>
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddDocumentsFromAsync<TLoader>(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        DataSource dataSource,
        ITextSplitter? textSplitter = null,
        DocumentLoaderSettings? loaderSettings = null,
        CancellationToken cancellationToken = default)
        where TLoader : IDocumentLoader, new()
    {
        var loader = new TLoader();
        var documents = await loader.LoadAsync(
            dataSource: dataSource,
            settings: loaderSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await vectorCollection.AddSplitDocumentsAsync(
            embeddingGenerator: embeddingGenerator,
            documents: documents,
            textSplitter: textSplitter,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Split documents and add them to the vector collection.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddSplitDocumentsAsync(
        this VectorStoreCollection<string, LangChainDocumentRecord> vectorCollection,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IReadOnlyCollection<Document> documents,
        ITextSplitter? textSplitter = null,
        CancellationToken cancellationToken = default)
    {
        textSplitter ??= new CharacterTextSplitter();

        var splitDocuments = textSplitter.SplitDocuments(documents);

        return await vectorCollection.AddDocumentsAsync(
            embeddingGenerator: embeddingGenerator,
            documents: splitDocuments,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

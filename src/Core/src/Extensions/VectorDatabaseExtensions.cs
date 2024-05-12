using LangChain.Databases;
using LangChain.Providers;
using LangChain.DocumentLoaders;
using LangChain.Splitters.Text;

namespace LangChain.Extensions;

public static class VectorDatabaseExtensions
{
    /// <summary>
    /// Adds documents loaded using <typeparamref name="TLoader"/> to the vector database. <br/>
    /// If the collection does not exist, it will be created. <br/>
    /// Splitting the text into sentences is done using the default text splitter, but you can provide your own. <br/>
    /// </summary>
    /// <param name="vectorDatabase"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="dimensions"></param>
    /// <param name="dataSource"></param>
    /// <param name="collectionName"></param>
    /// <param name="textSplitter"></param>
    /// <param name="loaderSettings"></param>
    /// <param name="embeddingSettings"></param>
    /// <param name="behavior"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<IVectorCollection> AddDocumentsFromAsync<TLoader>(
        this IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        int dimensions,
        DataSource dataSource,
        string collectionName = VectorCollection.DefaultName,
        ITextSplitter? textSplitter = null,
        DocumentLoaderSettings? loaderSettings = null,
        EmbeddingSettings? embeddingSettings = null,
        AddDocumentsToDatabaseBehavior behavior = AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists,
        CancellationToken cancellationToken = default)
        where TLoader : IDocumentLoader, new()
    {
        vectorDatabase = vectorDatabase ?? throw new ArgumentNullException(paramName: nameof(vectorDatabase));

        if (behavior == AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists &&
            await vectorDatabase.IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            return await vectorDatabase.GetCollectionAsync(
                collectionName: collectionName,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        if (behavior == AddDocumentsToDatabaseBehavior.OverwriteExistingCollection &&
            await vectorDatabase.IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            await vectorDatabase.DeleteCollectionAsync(
                collectionName: collectionName,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        
        var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync(
            collectionName: collectionName,
            dimensions: dimensions,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await vectorCollection.AddDocumentsFromAsync<TLoader>(
            embeddingModel: embeddingModel,
            dataSource: dataSource,
            textSplitter: textSplitter,
            loaderSettings: loaderSettings,
            embeddingSettings: embeddingSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return vectorCollection;
    }

    /// <summary>
    /// Adds documents loaded using <typeparamref name="TLoader"/> to the vector collection. <br/>
    /// Splitting the text into sentences is done using the default text splitter, but you can provide your own. <br/>
    /// </summary>
    /// <param name="vectorCollection"></param>
    /// <param name="embeddingModel"></param>
    /// <param name="dataSource"></param>
    /// <param name="textSplitter"></param>
    /// <param name="loaderSettings"></param>
    /// <param name="embeddingSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyCollection<string>> AddDocumentsFromAsync<TLoader>(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        DataSource dataSource,
        ITextSplitter? textSplitter = null,
        DocumentLoaderSettings? loaderSettings = null,
        EmbeddingSettings? embeddingSettings = null,
        CancellationToken cancellationToken = default)
        where TLoader : IDocumentLoader, new()
    {
        var loader = new TLoader();
        var documents = await loader.LoadAsync(
            dataSource: dataSource,
            settings: loaderSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await vectorCollection.AddSplitDocumentsAsync(
            embeddingModel: embeddingModel,
            documents: documents,
            textSplitter: textSplitter,
            embeddingSettings: embeddingSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a VectorDatabase table from documents.
    /// </summary>
    public static async Task<IReadOnlyCollection<string>> AddSplitDocumentsAsync(
        this IVectorCollection vectorCollection,
        IEmbeddingModel embeddingModel,
        IReadOnlyCollection<Document> documents,
        ITextSplitter? textSplitter = null,
        EmbeddingSettings? embeddingSettings = null,
        CancellationToken cancellationToken = default)
    {
        textSplitter ??= new CharacterTextSplitter();

        var splitDocuments = textSplitter.SplitDocuments(documents);

        return await vectorCollection.AddDocumentsAsync(
            embeddingModel: embeddingModel,
            documents: splitDocuments,
            embeddingSettings: embeddingSettings,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
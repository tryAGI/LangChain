using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch;

/// <summary>
/// Represents a vector store using OpenSearch.
/// </summary>
public class OpenSearchVectorStore : IVectorDatabaseExtended
{
    private readonly OpenSearchClient _client;
    private readonly string? _indexName;

    public static OpenSearchVectorStoreOptions? Options { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchVectorStore"/> class.
    /// </summary>
    /// <param name="options">The OpenSearch vector store options.</param>
    public OpenSearchVectorStore(OpenSearchVectorStoreOptions? options)
    {
        Options = options;
        _indexName = options?.IndexName;

        var settings = new ConnectionSettings(options?.ConnectionUri)
            .DefaultIndex(options?.IndexName)
            .BasicAuthentication(options?.Username, options?.Password);

        _client = new OpenSearchClient(settings);

        _ = GetOrCreateCollectionAsync(_indexName!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchVectorStore"/> class.
    /// </summary>
    /// <param name="indexName">The name of the Index.</param>
    public OpenSearchVectorStore(string indexName)
    {
        _indexName = indexName;
        _client = new OpenSearchClient();
    }

    /// <summary>
    /// Adds a collection of vector search items to the store.
    /// </summary>
    /// <param name="items">The collection of vector search items to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection of the added item IDs.</returns>
    public async Task<IReadOnlyCollection<string>> AddAsync(
        IReadOnlyCollection<VectorSearchItem> items,
        CancellationToken cancellationToken = default)
    {
        items = items ?? throw new ArgumentNullException(nameof(items));

        var bulkDescriptor = new BulkDescriptor();

        foreach (var item in items)
        {
            var content = item.Text.Trim();
            if (string.IsNullOrEmpty(content))
            {
                continue;
            }

            var vectorRecord = new VectorRecord
            {
                Id = item.Id,
                Text = content,
                Vector = item.Embedding ?? [],
                Metadata = (Dictionary<string, object>?)item.Metadata
            };

            bulkDescriptor.Index<VectorRecord>(
                indexDescriptor => indexDescriptor
                    .Document(vectorRecord)
                    .Index(_indexName)
            );
        }

        var bulkResponse = await _client.BulkAsync(bulkDescriptor, cancellationToken)
            .ConfigureAwait(false);

        return items
            .Select(i => i.Id)
            .ToArray();
    }

    /// <summary>
    /// Deletes items from the store by their IDs.
    /// </summary>
    /// <param name="ids">The IDs of the items to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether the deletion was successful.</returns>
    public Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        IList<bool> success = [];
        var enumerable = ids.ToList();
        foreach (var id in enumerable.ToList())
        {
            var request = new DeleteRequest(Indices.Index(_indexName), id);
            var response = _client.Delete(request);
            success.Add(response.IsValid);
        }

        return Task.FromResult(success.All(x => x));
    }

    /// <summary>
    /// Searches for vector records based on a search request.
    /// </summary>
    /// <param name="request">The search request.</param>
    /// <param name="settings">The search settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the search response.</returns>
    public async Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request,
        VectorSearchSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        settings ??= new VectorSearchSettings();

        var searchResponse = await _client.SearchAsync<VectorRecord>(s => s
            .Index(_indexName)
            .Query(q => q
                .Knn(knn => knn
                    .Field(f => f.Vector)
                    .Vector(request.Embeddings.First())
                    .K(settings.NumberOfResults)
                )
            ), cancellationToken).ConfigureAwait(false);

        return new VectorSearchResponse
        {
            Items = searchResponse.Documents
                .Where(vectorRecord => !string.IsNullOrWhiteSpace(vectorRecord.Text))
                .Select(vectorRecord => new VectorSearchItem
                {
                    Text = vectorRecord.Text ?? string.Empty,
                    Id = vectorRecord.Id,
                    Metadata = vectorRecord.Metadata,
                    Embedding = vectorRecord.Vector,
                })
                .ToArray()
        };
    }

    /// <summary>
    /// Creates a new collection with the specified options.
    /// </summary>
    /// <param name="options">The OpenSearch vector store options.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateCollectionAsync(OpenSearchVectorStoreOptions? options)
    {
        try
        {
            var createIndexResponse = _client.Indices.Create(options?.IndexName, c => c
                .Settings(x => x
                    .Setting("index.knn", true)
                    .Setting("index.knn.space_type", "cosinesimil")
                )
                .Map<VectorRecord>(m => m
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Nested<Dictionary<string, object>>(n => n.Name(x => x.Metadata))
                        .Text(t => t.Name(n => n.Text))
                        .KnnVector(d => d.Name(n => n.Vector).Dimension(options?.Dimensions).Similarity("cosine"))
                    )
                ));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a collection by its name.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the vector collection.</returns>
    public Task<VectorCollection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetIndexRequest(Indices.Index(collectionName)) ?? throw new InvalidOperationException("Collection not found");
            var collection = _client.Indices.Get(request);
            if (collection.IsValid == false) throw new InvalidOperationException("Collection not found");

            return Task.FromResult(new VectorCollection
            {
                Id = collectionName,
                Name = collectionName,
            });
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("Collection not found", innerException: exception);
        }
    }

    /// <summary>
    /// Deletes a collection by its name.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteCollectionAsync(string? collectionName, CancellationToken cancellationToken = default)
    {
        DeleteIndexRequest request = new(Indices.Index(collectionName));
        _client.Indices.Delete(request);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets or creates a collection by its name.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the vector collection.</returns>
    public async Task<VectorCollection> GetOrCreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        if (!await IsCollectionExistsAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            Options!.IndexName = collectionName;
            await CreateCollectionAsync(Options);
        }

        return await GetCollectionAsync(collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Checks if a collection exists.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether the collection exists.</returns>
    public Task<bool> IsCollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var existsResponse = _client.Indices.Exists(collectionName);
            return Task.FromResult(existsResponse.Exists);
        }
        catch (ArgumentNullException e)
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Gets an item by its ID from a specific collection.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="id">The ID of the item.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the vector search item.</returns>
    public Task<VectorSearchItem?> GetItemByIdAsync(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        var request = new GetRequest(Indices.Index(collectionName), id);
        var response = _client.Get<VectorRecord>(request);
        VectorRecord vectorRecord = response.Source;

        if (vectorRecord == null) return null!;

        return Task.FromResult(new VectorSearchItem
        {
            Id = vectorRecord.Id,
            Text = vectorRecord.Text!,
            Metadata = vectorRecord.Metadata
        })!;
    }
}
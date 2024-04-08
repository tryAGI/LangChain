using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch;

public class OpenSearchVectorStore : IVectorDatabase
{
    private readonly OpenSearchClient _client;
    private readonly string? _indexName;

    public static OpenSearchVectorStoreOptions DefaultOptions { get; } = new();

    public OpenSearchVectorStore(OpenSearchVectorStoreOptions? options)
    {
        options ??= DefaultOptions;
        _indexName = options.IndexName;

#pragma warning disable CA2000
        var settings = new ConnectionSettings(options.ConnectionUri)
#pragma warning restore CA2000
            .DefaultIndex(options.IndexName)
            .BasicAuthentication(options.Username, options.Password);

        _client = new OpenSearchClient(settings);

        var existsResponse = _client.Indices.Exists(_indexName);
        if (existsResponse.Exists == false)
        {
            CreateIndex(options);
        }
    }

    public OpenSearchVectorStore(string tableName)
    {
        _client = new OpenSearchClient();
    }

    /// <inheritdoc />
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
                Id = item.Id, //i++.ToString(CultureInfo.InvariantCulture),
                Text = content,
                Vector = item.Embedding ?? [],
            };

            bulkDescriptor.Index<VectorRecord>(
                indexDescriptor => indexDescriptor
                    .Document(vectorRecord)
                    .Index(_indexName)
            );
        }

        var bulkResponse = await _client!.BulkAsync(bulkDescriptor, cancellationToken)
            .ConfigureAwait(false);

        return items
            .Select(i => i.Id)
            .ToArray();
    }

    public Task<bool> DeleteAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
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
                    Embedding = vectorRecord.Vector,
                })
                .ToArray()
        };
    }

    private void CreateIndex(OpenSearchVectorStoreOptions options)
    {
        var createIndexResponse = _client.Indices.Create(options.IndexName, c => c
            .Settings(x => x
                .Setting("index.knn", true)
                .Setting("index.knn.space_type", "cosinesimil")
            )
        .Map<VectorRecord>(m => m
            .Properties(p => p
                .Keyword(k => k.Name(n => n.Id))
                .Text(t => t.Name(n => n.Text))
                .KnnVector(d => d.Name(n => n.Vector).Dimension(1536).Similarity("cosine"))
            )
        ));
    }
}
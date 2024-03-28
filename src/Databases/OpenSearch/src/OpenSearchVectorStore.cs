using System.Globalization;
using LangChain.Providers;
using LangChain.Sources;
using LangChain.VectorStores;
using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch;

public class OpenSearchVectorStore : VectorStore
{
    private readonly OpenSearchClient _client;
    private readonly string? _indexName;

    public static OpenSearchVectorStoreOptions DefaultOptions { get; } = new();

    public OpenSearchVectorStore(IEmbeddingModel embeddings,
        OpenSearchVectorStoreOptions? options)
        : base(embeddings)
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

    public OpenSearchVectorStore(string tableName, IEmbeddingModel embeddings)
        : base(embeddings)
    {
        _client = new OpenSearchClient();
    }

    public override async Task<IEnumerable<string>> AddDocumentsAsync(IEnumerable<Document> documents, CancellationToken cancellationToken = default)
    {
        var bulkDescriptor = new BulkDescriptor();
        var i = 1;

        var enumerable = documents as Document[] ?? documents.ToArray();
        foreach (var document in enumerable)
        {
            var content = document.PageContent.Trim();
            if (string.IsNullOrEmpty(content)) continue;

            var embed = await EmbeddingModel.CreateEmbeddingsAsync(document.PageContent, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var vectorRecord = new VectorRecord
            {
                Id = i++.ToString(CultureInfo.InvariantCulture),
                Text = document.PageContent,
                Vector = embed.Values.SelectMany(x => x).ToArray()
            };

            bulkDescriptor.Index<VectorRecord>(desc => desc
                .Document(vectorRecord)
                .Index(_indexName)
            );
        }

        var bulkResponse = await _client!.BulkAsync(bulkDescriptor, cancellationToken)
            .ConfigureAwait(false);

        return enumerable.Select(x => x.PageContent);
    }

    public override Task<IEnumerable<string>> AddTextsAsync(IEnumerable<string> texts, IEnumerable<Dictionary<string, object>>? metadatas = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(
        IEnumerable<float> embedding,
        int k = 4,
        CancellationToken cancellationToken = default)
    {
        var searchResponse = await _client.SearchAsync<VectorRecord>(s => s
            .Index(_indexName)
            .Query(q => q
                .Knn(knn => knn
                    .Field(f => f.Vector)
                    .Vector(embedding.ToArray())
                    .K(k)
                )
            ), cancellationToken).ConfigureAwait(false);

        var documents = searchResponse.Documents
            .Where(vectorRecord => !string.IsNullOrWhiteSpace(vectorRecord.Text))
            .Select(vectorRecord => new Document
            {
                PageContent = vectorRecord.Text!,
            })
            .ToArray();

        return documents;
    }

    public override Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(string query, int k = 4, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(IEnumerable<float> embedding, int k = 4, int fetchK = 20, float lambdaMult = default,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(string query, int k = 4, int fetchK = 20, float lambdaMult = default,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override Func<float, float> SelectRelevanceScoreFn()
    {
        throw new NotImplementedException();
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
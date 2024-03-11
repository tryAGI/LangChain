using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch.Tests;

//
// docker run -p 9200:9200 -p 9600:9600 -e "discovery.type=single-node" -e "plugins.security.disabled=true" -e "OPENSEARCH_INITIAL_ADMIN_PASSWORD=<custom-admin-password>" opensearchproject/opensearch:latest
//
public class BasicOpenSearchTests
{
    OpenSearchClient? _client;
    private string? _indexName;
    public List<VectorRecord> _documents;

    [SetUp]
    public void Setup()
    {
        var password = Environment.GetEnvironmentVariable("OPENSEARCH_INITIAL_ADMIN_PASSWORD");
        _indexName = "my-index";

        var uri = new Uri("https://search-myopensearch-dvy73fcpabi2dch3xtxvxxooau.aos.us-east-1.on.aws");
        var settings = new ConnectionSettings(uri)
            .DefaultIndex(_indexName)
            .BasicAuthentication("taugustj", password);
        _client = new OpenSearchClient(settings);
    }

    [TearDown]
    public async Task TearDown()
    {
        var response = await _client.Indices.DeleteAsync(_indexName);
        var isValid = response.IsValid;
    }

    [Test]
    public async Task can_create_opensearch()
    {
        var response = await _client.PingAsync().ConfigureAwait(false);
        var isValid = response.IsValid;

        var createIndexResponse = _client.Indices.Create(_indexName, c => c
            .Settings(x => x
                .Setting("index.knn", true)
                .Setting("index.knn.space_type", "cosinesimil")
            )
            .Map<VectorRecord>(m => m
                .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Text(t => t.Name(n => n.Text))
                        .KnnVector(d => d.Name(n => n.Vector).Dimension(4).Similarity("cosine")) // Specify the vector dimensions
                )
            )
        );
        isValid = createIndexResponse.IsValid;

        _documents =
        [
            new VectorRecord { Id = "1", Text = "Sample text 1", Vector = new float[] { 0.1f, 0.2f, 0.3f, 0.9f } },
            new VectorRecord { Id = "2", Text = "Sample text 2", Vector = new float[] { 0.2f, 0.3f, 0.4f, 1.0f } },
            new VectorRecord { Id = "3", Text = "Sample text 3", Vector = new float[] { 0.5f, 0.3f, 0.4f, 1.0f } },
            new VectorRecord { Id = "4", Text = "Sample text 4", Vector = new float[] { 0.4f, 0.3f, 0.4f, 1.0f } }
        ];

        var bulkDescriptor = new BulkDescriptor();
        foreach (var doc in _documents)
        {
            bulkDescriptor.Index<VectorRecord>(i => i
                .Document(doc)
                .Index(_indexName)
            );
        }
        var bulkResponse = await _client.BulkAsync(bulkDescriptor);
        isValid = bulkResponse.IsValid;

        var searchResponse = _client.Search<VectorRecord>(s => s
            .Index(_indexName)
            .Query(q => q
                .Knn(k => k
                        .Field(f => f.Vector)
                        .Vector(new float[] { 0.8f, 0.7f, 0.8f, 0.9f }) // Replace with your query vector
                        .K(10) // Number of nearest neighbors to retrieve
                )
            )
        );
        isValid = searchResponse.IsValid;

        var similarDocuments = searchResponse.Documents;

        Console.WriteLine($"{isValid} - {similarDocuments.Count}");
    }
}
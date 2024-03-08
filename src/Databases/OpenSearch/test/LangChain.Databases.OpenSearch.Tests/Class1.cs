using OpenSearch.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Databases.OpenSearch.Tests
{
    public class MyDocument
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public float[] Vector { get; set; }
    }

    public class OpenSearchTests2
    {
        [Test]
        public async Task can_create_opensearch()
        {
            var password = Environment.GetEnvironmentVariable("OPENSEARCH_INITIAL_ADMIN_PASSWORD");

            var indexName = "my-index";
            var uri = new Uri("http://127.0.0.1:9200 "); // Replace with your OpenSearch URL
            var settings = new ConnectionSettings(uri)
                //      .DefaultIndex(indexName)
               // .BasicAuthentication("admin", password)
                ;
            var client = new OpenSearchClient(settings);

            var response = await client.PingAsync().ConfigureAwait(false);
            var isValid = response.IsValid;

            var createIndexResponse = client.Indices.Create(indexName, c => c
                .Map<MyDocument>(m => m
                    .Properties(p => p
                            .Keyword(k => k.Name(n => n.Id))
                            .Text(t => t.Name(n => n.Text))
                            .KnnVector(d => d.Name(n => n.Vector).Dimension(100)) // Specify the vector dimensions
                    )
                )
            );

            var document = new MyDocument
            {
                Id = "1",
                Text = "Sample text",
                Vector = new float[] { 0.1f, 0.2f, 0.3f, 0.9f } // Replace with your vector values
            };

            var indexResponse = client.Index(document, i => i.Index(indexName));


            var searchResponse = client.Search<MyDocument>(s => s
                .Index(indexName)
                .Query(q => q
                    .Knn(k => k
                            .Field(f => f.Vector)
                            .Vector(new float[] { 0.2f, 0.3f, 0.4f, 1.0f }) // Replace with your query vector
                            .K(10) // Number of nearest neighbors to retrieve
                    )
                )
            );

            var similarDocuments = searchResponse.Documents;
        }
    }
}

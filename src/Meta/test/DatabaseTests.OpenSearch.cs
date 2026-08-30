using DotNet.Testcontainers.Builders;
using LangChain.Databases.OpenSearch;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using static LangChain.Chains.Chain;

namespace LangChain.Databases.IntegrationTests;

public partial class OpenSearchTests
{
    #region Query Simple Documents

    private static async Task<(DatabaseTestEnvironment Environment, OpenSearchVectorStore VectorStore)> StartEnvironmentAsync()
    {
        const string password = "StronG#1235";

        var port1 = Random.Shared.Next(49152, 65535);
        var port2 = Random.Shared.Next(49152, 65535);
        var container = new ContainerBuilder("opensearchproject/opensearch:latest")
            .WithPortBinding(hostPort: port1, containerPort: 9600)
            .WithPortBinding(hostPort: port2, containerPort: 9200)
            .WithEnvironment("discovery.type", "single-node")
            .WithEnvironment("plugins.security.disabled", "true")
            .WithEnvironment("OPENSEARCH_INITIAL_ADMIN_PASSWORD", password)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(9200, ws => ws.WithTimeout(TimeSpan.FromMinutes(2))))
            .Build();

        await container.StartAsync();

        var vectorStore = new OpenSearchVectorStore(new OpenSearchVectorDatabaseOptions
        {
            ConnectionUri = new Uri($"http://localhost:{port2}"),
            Username = "admin",
            Password = password,
        });

        var environment = new DatabaseTestEnvironment
        {
            Container = container,
            Port = port2,
        };

        return (environment, vectorStore);
    }

    private static async Task<(DatabaseTestEnvironment Environment, OpenSearchVectorStore VectorStore)> SetupDocumentTestsAsync()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") is { Length: > 0 } key ? key :
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");

        var (environment, vectorStore) = await StartEnvironmentAsync();
        environment.Dimensions = 1536;

        var openAiClient = new OpenAI.OpenAIClient(apiKey);
        environment.EmbeddingGenerator = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

        return (environment, vectorStore);
    }

    [Test]
    [Explicit("Requires OPENAI_API_KEY and Docker")]
    public async Task index_test_documents()
    {
        var (environment, vectorStore) = await SetupDocumentTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        var texts = new[]
        {
            "I spent entire day watching TV",
            "My dog's name is Bob",
            "The car is orange",
            "This icecream is delicious",
            "It is cold in space",
        };

        await vectorCollection.AddTextsAsync(environment.EmbeddingGenerator!, texts);

        Console.WriteLine("pages: " + texts.Length);
    }

    [Test]
    [Explicit("Requires OPENAI_API_KEY and Docker")]
    public async Task can_query_test_documents()
    {
        var (environment, vectorStore) = await SetupDocumentTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        const string question = "what color is the car?";
        var results = await vectorCollection.GetSimilarDocuments(environment.EmbeddingGenerator!, question, amount: 2);

        Console.WriteLine("Similar documents: " + results.AsString());
    }

    #endregion

    #region Query Pdf Book

    [Test]
    [Explicit("Requires OPENAI_API_KEY and Docker")]
    public async Task index_harry_potter_book()
    {
        var (environment, vectorStore) = await SetupDocumentTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        var pdfSource = new PdfPigPdfLoader();
        var documents = await pdfSource.LoadAsync(DataSource.FromPath("x:\\Harry-Potter-Book-1.pdf"));

        var pages = documents.Select(d => d.PageContent).ToArray();
        await vectorCollection.AddTextsAsync(environment.EmbeddingGenerator!, pages);

        Console.WriteLine("pages: " + pages.Length);
    }

    [Test]
    [Explicit("Requires OPENAI_API_KEY and Docker")]
    public async Task can_query_harry_potter_book()
    {
        var (environment, vectorStore) = await SetupDocumentTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        const string question = "Who was drinking a unicorn blood?";
        var results = await vectorCollection.GetSimilarDocuments(environment.EmbeddingGenerator!, question, amount: 10);

        Console.WriteLine("Similar documents: " + results.AsString());
    }

    #endregion
}

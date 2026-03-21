using DotNet.Testcontainers.Builders;
using LangChain.Databases.OpenSearch;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Providers.Amazon.Bedrock.Predefined.Anthropic;
using LangChain.DocumentLoaders;
using LangChain.Schema;
using Microsoft.Extensions.VectorData;
using static LangChain.Chains.Chain;

namespace LangChain.Databases.IntegrationTests;

public partial class OpenSearchTests
{
    #region Query Images

    private static async Task<(DatabaseTestEnvironment Environment, OpenSearchVectorStore VectorStore)> SetupImageTestsAsync()
    {
        var (environment, vectorStore) = await StartEnvironmentAsync();
        environment.Dimensions = 1024;
        environment.EmbeddingModel = new TitanEmbedImageV1Model(new BedrockProvider())
        {
            Settings = new BedrockEmbeddingSettings
            {
                Dimensions = environment.Dimensions,
            }
        };

        return (environment, vectorStore);
    }

    [Test]
    [Explicit]
    public async Task index_test_images()
    {
        var (environment, vectorStore) = await SetupImageTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        string[] extensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tiff" };
        var files = Directory.EnumerateFiles(@"[images directory]", "*.*", SearchOption.AllDirectories)
            .Where(s => extensions.Any(ext => ext == Path.GetExtension(s)));

        var images = files.ToBinaryData();

        foreach (BinaryData image in images)
        {
            var model = new Claude3HaikuModel(new BedrockProvider());
            var message = new Message(" \"what's this an image of and describe the details?\"", MessageRole.Human);

            var chatRequest = ChatRequest.ToChatRequest(message);
            chatRequest.Image = image;

            var response = await model.GenerateAsync(chatRequest);

            var embeddingResponse = await environment.EmbeddingModel!.CreateEmbeddingsAsync(
                new EmbeddingRequest { Strings = [response] });

            await vectorCollection.UpsertAsync(new LangChainDocumentRecord
            {
                Text = response,
                Embedding = embeddingResponse.Values[0],
            });
        }
    }

    [Test]
    [Explicit]
    public async Task can_query_image_against_images()
    {
        var (environment, vectorStore) = await SetupImageTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        var path = Path.Combine(Path.GetTempPath(), "test_image.jpg");
        var imageData = await File.ReadAllBytesAsync(path);
        var binaryData = new BinaryData(imageData, "image/jpg");

        var embeddingRequest = new EmbeddingRequest
        {
            Strings = new List<string>(),
            Images = new List<Data> { Data.FromBytes(binaryData.ToArray()) }
        };
        var embedding = await environment.EmbeddingModel!.CreateEmbeddingsAsync(embeddingRequest)
            .ConfigureAwait(false);

        var floats = embedding.ToSingleArray();
        var results = new List<VectorSearchResult<LangChainDocumentRecord>>();
        await foreach (var result in vectorCollection.SearchAsync(new ReadOnlyMemory<float>(floats), top: 10).ConfigureAwait(false))
        {
            results.Add(result);
        }

        Console.WriteLine("Count: " + results.Count);
    }

    [Test]
    [Explicit]
    public async Task can_query_text_against_images()
    {
        var (environment, vectorStore) = await SetupImageTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        // Note: This test requires IEmbeddingGenerator (MEAI). Bedrock models implement IEmbeddingModel only.
        // When Bedrock models add MEAI support, use RetrieveDocuments(vectorCollection, embeddingGenerator, ...)
        Console.WriteLine("Test requires MEAI-compatible embedding model. Skipping chain execution.");
    }

    #endregion

    #region Query Simple Documents

    private static async Task<(DatabaseTestEnvironment Environment, OpenSearchVectorStore VectorStore)> StartEnvironmentAsync()
    {
        const string password = "StronG#1235";

        var port1 = Random.Shared.Next(49152, 65535);
        var port2 = Random.Shared.Next(49152, 65535);
        var container = new ContainerBuilder("opensearchproject/opensearch:latest")
            .WithPortBinding(hostPort: port1, containerPort: 9600) // multiple ports can be not supported
            .WithPortBinding(hostPort: port2, containerPort: 9200) // multiple ports can be not supported
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
        var (environment, vectorStore) = await StartEnvironmentAsync();
        environment.Dimensions = 1536;
        environment.EmbeddingModel = new TitanEmbedTextV1Model(new BedrockProvider())
        {
            Settings = new BedrockEmbeddingSettings
            {
                Dimensions = environment.Dimensions,
            }
        };

        return (environment, vectorStore);
    }

    [Test]
    [Explicit]
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

        var embeddingResponse = await environment.EmbeddingModel!.CreateEmbeddingsAsync(
            new EmbeddingRequest { Strings = texts.ToList() });

        for (var i = 0; i < texts.Length; i++)
        {
            await vectorCollection.UpsertAsync(new LangChainDocumentRecord
            {
                Text = texts[i],
                Embedding = embeddingResponse.Values[i],
            });
        }

        Console.WriteLine("pages: " + texts.Length);
    }

    [Test]
    [Explicit]
    public async Task can_query_test_documents()
    {
        var (environment, vectorStore) = await SetupDocumentTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        const string question = "what color is the car?";
        var embeddingResponse = await environment.EmbeddingModel!.CreateEmbeddingsAsync(
            new EmbeddingRequest { Strings = [question] });

        var results = new List<Document>();
        await foreach (var result in vectorCollection.SearchAsync(
            new ReadOnlyMemory<float>(embeddingResponse.Values[0]),
            top: 2))
        {
            results.Add(new Document(result.Record.Text ?? string.Empty));
        }

        Console.WriteLine("Similar documents: " + results.AsString());
    }

    #endregion

    #region Query Pdf Book

    [Test]
    [Explicit]
    public async Task index_harry_potter_book()
    {
        var (environment, vectorStore) = await SetupDocumentTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        var pdfSource = new PdfPigPdfLoader();
        var documents = await pdfSource.LoadAsync(DataSource.FromPath("x:\\Harry-Potter-Book-1.pdf"));

        var texts = documents.Select(d => d.PageContent).ToList();
        var embeddingResponse = await environment.EmbeddingModel!.CreateEmbeddingsAsync(
            new EmbeddingRequest { Strings = texts });

        var count = 0;
        for (var i = 0; i < texts.Count; i++)
        {
            await vectorCollection.UpsertAsync(new LangChainDocumentRecord
            {
                Text = texts[i],
                Embedding = embeddingResponse.Values[i],
            });
            count++;
        }

        Console.WriteLine("pages: " + count);
    }

    [Test]
    [Explicit]
    public async Task can_query_harry_potter_book()
    {
        var (environment, vectorStore) = await SetupDocumentTestsAsync();
        await using var _ = environment;
        var vectorCollection = vectorStore.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        const string question = "Who was drinking a unicorn blood?";
        var embeddingResponse = await environment.EmbeddingModel!.CreateEmbeddingsAsync(
            new EmbeddingRequest { Strings = [question] });

        var results = new List<Document>();
        await foreach (var result in vectorCollection.SearchAsync(
            new ReadOnlyMemory<float>(embeddingResponse.Values[0]),
            top: 10))
        {
            results.Add(new Document(result.Record.Text ?? string.Empty));
        }

        Console.WriteLine("Similar documents: " + results.AsString());
    }

    #endregion
}

using LangChain.Sources;
using LangChain.Providers;
using Moq;
using Newtonsoft.Json;
using Npgsql;

namespace LangChain.Databases.Postgres.IntegrationTests;

/// <summary>
/// In order to run tests please run postgres with installed pgvector locally
/// e.g. with docker <see href="https://github.com/pgvector/pgvector#additional-installation-methods"/>
/// docker run -p 5433:5432 -e POSTGRES_PASSWORD=password -e POSTGRES_DB=test ankane/pgvector
/// </summary>
[TestFixture]
[Explicit]
public class PostgresTests
{
    private Dictionary<string, float[]> EmbeddingsDict { get; } = new();
    private readonly string _connectionString;

    public PostgresTests()
    {
        const string host = "localhost";
        const int port = 5433;

        _connectionString = $"Host={host};Port={port};Database=test;User ID=postgres;Password=password;";

        PopulateEmbedding();
        EnsureVectorExtensionAsync();
    }

    [Test]
    public async Task CreateAndDeleteCollection_Ok()
    {
        var tableName = "CreateAndDeleteCollection_Ok";
        
        var db = new PostgresDbClient(_connectionString, "public", 1536);

        await db.CreateEmbeddingTableAsync(tableName);
        var exists = await db.IsTableExistsAsync(tableName);

        exists.Should().BeTrue();

        await db.DropTableAsync(tableName);
        
        exists = await db.IsTableExistsAsync(tableName);
        
        exists.Should().BeFalse();
    }

    [Test]
    public async Task AddDocuments_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();

        var db = new PostgresDbClient(_connectionString, "public", 1536);
        await db.CreateEmbeddingTableAsync(collectionName);
        var store = new PostgresVectorStore(_connectionString, 1526, embeddingsMock.Object, collectionName: collectionName);

        var documents = new[]
        {
            new Document("apple", new Dictionary<string, object>
            {
                ["color"] = "red"
            }),
            new Document("orange", new Dictionary<string, object>
            {
                ["color"] = "orange"
            })
        };

        var ids = await store.AddDocumentsAsync(documents);

        ids.Should().HaveCount(2);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        firstId.Should().NotBeEmpty();
        secondId.Should().NotBeEmpty();

        var actualFirstDocument = await store.GetDocumentByIdAsync(firstId);
        actualFirstDocument.Should().NotBeNull();
        actualFirstDocument!.PageContent.Should().BeEquivalentTo(documents[0].PageContent);
        actualFirstDocument.Metadata["color"].Should().BeEquivalentTo(documents[0].Metadata["color"]);

        var actualSecondDocument = await store.GetDocumentByIdAsync(secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument!.PageContent.Should().BeEquivalentTo(documents[1].PageContent);
        actualSecondDocument.Metadata["color"].Should().BeEquivalentTo(documents[1].Metadata["color"]);
    }

    [Test]
    public async Task AddTexts_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();

        var db = new PostgresDbClient(_connectionString, "public", 1536);
        await db.CreateEmbeddingTableAsync(collectionName);
        var store = new PostgresVectorStore(_connectionString, 1526, embeddingsMock.Object, collectionName: collectionName);

        var texts = new[] { "apple", "orange" };
        var metadatas = new Dictionary<string, object>[2];
        metadatas[0] = new Dictionary<string, object>
        {
            ["string"] = "red",
            ["double"] = 1.01d,
            ["guid"] = 1.01d,
        };

        metadatas[1] = new Dictionary<string, object>
        {
            ["color"] = "orange"
        };

        var ids = await store.AddTextsAsync(texts, metadatas);

        ids.Should().HaveCount(2);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        firstId.Should().NotBeEmpty();
        secondId.Should().NotBeEmpty();

        var actualFirstDocument = await store.GetDocumentByIdAsync(firstId);
        actualFirstDocument.Should().NotBeNull();
        actualFirstDocument!.PageContent.Should().BeEquivalentTo(texts[0]);
        actualFirstDocument.Metadata["string"].Should().BeEquivalentTo(metadatas[0]["string"]);
        actualFirstDocument.Metadata["double"].Should().BeEquivalentTo(metadatas[0]["double"]);
        actualFirstDocument.Metadata["guid"].Should().BeEquivalentTo(metadatas[0]["guid"]);

        var actualSecondDocument = await store.GetDocumentByIdAsync(secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument!.PageContent.Should().BeEquivalentTo(texts[1]);
        actualSecondDocument.Metadata["color"].Should().BeEquivalentTo(metadatas[1]["color"]);
    }

    [Test]
    public async Task DeleteDocuments_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();

        var db = new PostgresDbClient(_connectionString, "public", 1536);
        await db.CreateEmbeddingTableAsync(collectionName);
        var store = new PostgresVectorStore(_connectionString, 1526, embeddingsMock.Object, collectionName: collectionName);

        var documents = new[]
        {
            new Document("apple", new Dictionary<string, object>
            {
                ["color"] = "red"
            }),
            new Document("orange", new Dictionary<string, object>
            {
                ["color"] = "orange"
            })
        };

        var ids = await store.AddDocumentsAsync(documents);

        await store.DeleteAsync(ids);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        var actualFist = await store.GetDocumentByIdAsync(firstId);
        var actualSecond = await store.GetDocumentByIdAsync(secondId);

        actualFist.Should().BeNull();
        actualSecond.Should().BeNull();
    }

    [Test]
    public async Task SimilaritySearch_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();

        var db = new PostgresDbClient(_connectionString, "public", 1536);
        await db.CreateEmbeddingTableAsync(collectionName);
        var store = new PostgresVectorStore(_connectionString, 1526, embeddingsMock.Object, collectionName: collectionName);

        await store.AddTextsAsync(EmbeddingsDict.Keys);

        var similar = await store.SimilaritySearchAsync("lemon", k: 5);
        similar.Should().HaveCount(5);

        var similarTexts = similar.Select(s => s.PageContent).ToArray();

        similarTexts[0].Should().BeEquivalentTo("lemon");
        similarTexts.Should().Contain("orange");
        similarTexts.Should().Contain("peach");
        similarTexts.Should().Contain("banana");
        similarTexts.Should().Contain("apple");
    }

    [Test]
    public async Task SimilaritySearchByVector_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();

        var db = new PostgresDbClient(_connectionString, "public", 1536);
        await db.CreateEmbeddingTableAsync(collectionName);
        var store = new PostgresVectorStore(_connectionString, 1526, embeddingsMock.Object, collectionName: collectionName);

        await store.AddTextsAsync(EmbeddingsDict.Keys);

        var similar = await store.SimilaritySearchByVectorAsync(EmbeddingsDict["lemon"], k: 5);
        similar.Should().HaveCount(5);

        var similarTexts = similar.Select(s => s.PageContent).ToArray();

        similarTexts[0].Should().BeEquivalentTo("lemon");
        similarTexts.Should().Contain("orange");
        similarTexts.Should().Contain("peach");
        similarTexts.Should().Contain("banana");
        similarTexts.Should().Contain("apple");
    }

    [Test]
    public async Task SimilaritySearchWithScores_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();

        var db = new PostgresDbClient(_connectionString, "public", 1536);
        await db.CreateEmbeddingTableAsync(collectionName);
        var store = new PostgresVectorStore(_connectionString, 1526, embeddingsMock.Object, collectionName: collectionName);

        await store.AddTextsAsync(EmbeddingsDict.Keys);

        var similar = await store.SimilaritySearchWithScoreAsync("lemon", k: 5);
        similar.Should().HaveCount(5);

        var first = similar.First();

        first.Item1.PageContent.Should().BeEquivalentTo("lemon");
        first.Item2.Should().BeGreaterOrEqualTo(1f);
    }

    private void PopulateEmbedding()
    {
        foreach (var embeddingFile in Directory.EnumerateFiles("embeddings"))
        {
            var jsonRaw = File.ReadAllText(embeddingFile);
            var json =
                JsonConvert.DeserializeObject<Dictionary<string, float[]>>(jsonRaw) ??
                throw new InvalidOperationException("json is null");
            var kv = json.First();
            EmbeddingsDict.Add(kv.Key, kv.Value);
        }
    }

    private static string GenerateCollectionName() => "test-" + Guid.NewGuid().ToString("N");

    private void EnsureVectorExtensionAsync()
    {
        var dataSource = new NpgsqlDataSourceBuilder(_connectionString).Build();
        var connection = dataSource.OpenConnection();
        using (connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "CREATE EXTENSION IF NOT EXISTS vector";

            command.ExecuteNonQuery();
        }
    }

    private Mock<IEmbeddingModel> CreateFakeEmbeddings()
    {
        var mock = new Mock<IEmbeddingModel>();

        mock.Setup(x => x.CreateEmbeddingsAsync(
                It.IsAny<string>(),
                It.IsAny<EmbeddingSettings>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, EmbeddingSettings, CancellationToken>(
                (query, _, _) =>
                {
                    var embedding = EmbeddingsDict.TryGetValue(query, out var value)
                        ? value
                        : throw new ArgumentException("not in dict");

                    return Task.FromResult(new EmbeddingResponse
                    {
                        Values = [embedding],
                        Usage = Usage.Empty,
                        UsedSettings = EmbeddingSettings.Default,
                    });
                });

        mock.Setup(x => x.CreateEmbeddingsAsync(
            It.IsAny<string[]>(),
            It.IsAny<EmbeddingSettings>(),
            It.IsAny<CancellationToken>()))
            .Returns<string[], EmbeddingSettings, CancellationToken>(
                (texts, _, _) =>
                {
                    var embeddings = new float[texts.Length][];

                    for (int index = 0; index < texts.Length; index++)
                    {
                        var text = texts[index];
                        embeddings[index] = EmbeddingsDict.TryGetValue(text, out var value)
                            ? value
                            : throw new ArgumentException("not in dict");
                    }

                    return Task.FromResult(new EmbeddingResponse
                    {
                        Values = embeddings,
                        Usage = Usage.Empty,
                        UsedSettings = EmbeddingSettings.Default,
                    });
                });

        return mock;
    }
}
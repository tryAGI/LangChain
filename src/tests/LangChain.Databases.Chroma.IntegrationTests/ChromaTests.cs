using LangChain.Abstractions.Embeddings.Base;
using LangChain.Docstore;
using Microsoft.SemanticKernel.Connectors.Memory.Chroma;
using Moq;
using Newtonsoft.Json;

namespace LangChain.Databases.Chroma.IntegrationTests;

/// <summary>
/// In order to run tests please run chroma locally, e.g. with docker
/// docker run -p 8000:8000 chromadb/chroma
/// </summary>
[TestClass]
#if CONTINUOUS_INTEGRATION_BUILD
[Ignore]
#endif
public class ChromaTests
{
    public Dictionary<string, float[]> EmbeddingsDict { get; } = new();

    public ChromaTests()
    {
        PopulateEmbedding();
    }

    [TestMethod]
    public async Task CreateAndDeleteCollection_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();
        var chroma = new ChromaVectorStore(httpClient, "http://localhost:8000", embeddingsMock.Object, collectionName);

        var actual = await chroma.GetCollectionAsync();

        actual.Should().NotBeNull();
        actual.Id.Should().NotBeEmpty();
        actual.Name.Should().BeEquivalentTo(collectionName);

        await chroma.DeleteCollectionAsync();

        await Assert.ThrowsExceptionAsync<ChromaClientException>(() => chroma.GetCollectionAsync());
    }

    [TestMethod]
    public async Task AddDocuments_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();
        var chroma = new ChromaVectorStore(httpClient, "http://localhost:8000", embeddingsMock.Object, collectionName);

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

        var ids = await chroma.AddDocumentsAsync(documents);

        ids.Should().HaveCount(2);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        firstId.Should().NotBeEmpty();
        secondId.Should().NotBeEmpty();

        var actualFirstDocument = await chroma.GetDocumentByIdAsync(firstId);
        actualFirstDocument.Should().NotBeNull();
        actualFirstDocument.PageContent.Should().BeEquivalentTo(documents[0].PageContent);
        actualFirstDocument.Metadata["color"].Should().BeEquivalentTo(documents[0].Metadata["color"]);

        var actualSecondDocument = await chroma.GetDocumentByIdAsync(secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument.PageContent.Should().BeEquivalentTo(documents[1].PageContent);
        actualSecondDocument.Metadata["color"].Should().BeEquivalentTo(documents[1].Metadata["color"]);
    }

    [TestMethod]
    public async Task AddTexts_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();
        var chroma = new ChromaVectorStore(httpClient, "http://localhost:8000", embeddingsMock.Object, collectionName);

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

        var ids = await chroma.AddTextsAsync(texts, metadatas);

        ids.Should().HaveCount(2);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        firstId.Should().NotBeEmpty();
        secondId.Should().NotBeEmpty();

        var actualFirstDocument = await chroma.GetDocumentByIdAsync(firstId);
        actualFirstDocument.Should().NotBeNull();
        actualFirstDocument.PageContent.Should().BeEquivalentTo(texts[0]);
        actualFirstDocument.Metadata["string"].Should().BeEquivalentTo(metadatas[0]["string"]);
        actualFirstDocument.Metadata["double"].Should().BeEquivalentTo(metadatas[0]["double"]);
        actualFirstDocument.Metadata["guid"].Should().BeEquivalentTo(metadatas[0]["guid"]);

        var actualSecondDocument = await chroma.GetDocumentByIdAsync(secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument.PageContent.Should().BeEquivalentTo(texts[1]);
        actualSecondDocument.Metadata["color"].Should().BeEquivalentTo(metadatas[1]["color"]);
    }

    [TestMethod]
    public async Task DeleteDocuments_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();
        var chroma = new ChromaVectorStore(httpClient, "http://localhost:8000", embeddingsMock.Object, collectionName);

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

        var ids = await chroma.AddDocumentsAsync(documents);

        await chroma.DeleteAsync(ids);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        var actualFist = await chroma.GetDocumentByIdAsync(firstId);
        var actualSecond = await chroma.GetDocumentByIdAsync(secondId);

        actualFist.Should().BeNull();
        actualSecond.Should().BeNull();
    }

    [TestMethod]
    public async Task SimilaritySearch_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();
        var chroma = new ChromaVectorStore(httpClient, "http://localhost:8000", embeddingsMock.Object, collectionName);

        await chroma.AddTextsAsync(EmbeddingsDict.Keys);

        var similar = await chroma.SimilaritySearchAsync("lemon", k: 5);
        similar.Should().HaveCount(5);

        var similarTexts = similar.Select(s => s.PageContent).ToArray();

        similarTexts[0].Should().BeEquivalentTo("lemon");
        similarTexts.Should().Contain("orange");
        similarTexts.Should().Contain("peach");
        similarTexts.Should().Contain("banana");
        similarTexts.Should().Contain("apple");
    }

    [TestMethod]
    public async Task SimilaritySearchByVector_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();
        var chroma = new ChromaVectorStore(httpClient, "http://localhost:8000", embeddingsMock.Object, collectionName);

        await chroma.AddTextsAsync(EmbeddingsDict.Keys);

        var similar = await chroma.SimilaritySearchByVectorAsync(EmbeddingsDict["lemon"], k: 5);
        similar.Should().HaveCount(5);

        var similarTexts = similar.Select(s => s.PageContent).ToArray();

        similarTexts[0].Should().BeEquivalentTo("lemon");
        similarTexts.Should().Contain("orange");
        similarTexts.Should().Contain("peach");
        similarTexts.Should().Contain("banana");
        similarTexts.Should().Contain("apple");
    }

    [TestMethod]
    public async Task SimilaritySearchWithScores_Ok()
    {
        using var httpClient = new HttpClient();
        var embeddingsMock = CreateFakeEmbeddings();
        var collectionName = GenerateCollectionName();
        var chroma = new ChromaVectorStore(httpClient, "http://localhost:8000", embeddingsMock.Object, collectionName);

        await chroma.AddTextsAsync(EmbeddingsDict.Keys);

        var similar = await chroma.SimilaritySearchWithScoreAsync("lemon", k: 5);
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
            var json = JsonConvert.DeserializeObject<Dictionary<string, float[]>>(jsonRaw);
            var kv = json.First();
            EmbeddingsDict.Add(kv.Key, kv.Value);
        }
    }

    private static string GenerateCollectionName() => "test-" + Guid.NewGuid().ToString("N");

    private Mock<IEmbeddings> CreateFakeEmbeddings()
    {
        var mock = new Mock<IEmbeddings>();

        mock.Setup(x => x.EmbedQueryAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>(
                (query, _) =>
                {
                    var embedding = EmbeddingsDict.TryGetValue(query, out var value)
                        ? value
                        : throw new ArgumentException("not in dict");

                    return Task.FromResult(embedding);
                });

        mock.Setup(x => x.EmbedDocumentsAsync(
            It.IsAny<string[]>(),
            It.IsAny<CancellationToken>()))
            .Returns<string[], CancellationToken>(
                (texts, _) =>
                {
                    var embeddings = new float[texts.Length][];

                    for (int index = 0; index < texts.Length; index++)
                    {
                        var text = texts[index];
                        embeddings[index] = EmbeddingsDict.TryGetValue(text, out var value)
                            ? value
                            : throw new ArgumentException("not in dict");
                    }

                    return Task.FromResult(embeddings);
                });

        return mock;
    }
}
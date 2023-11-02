using LangChain.Abstractions.Embeddings.Base;
using LangChain.Docstore;
using Microsoft.SemanticKernel.Connectors.Memory.Chroma;
using Moq;

namespace LangChain.Databases.Chroma.IntegrationTests;

[TestClass]
public class ChromaTests
{
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
        actualFirstDocument.Metadata["color"].ToString().Should().BeEquivalentTo(documents[0].Metadata["color"].ToString());
        
        var actualSecondDocument = await chroma.GetDocumentByIdAsync(secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument.PageContent.Should().BeEquivalentTo(documents[1].PageContent);
        actualSecondDocument.Metadata["color"].ToString().Should().BeEquivalentTo(documents[1].Metadata["color"].ToString());
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
            ["color"] = "red"
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
        actualFirstDocument.Metadata["color"].ToString().Should().BeEquivalentTo(metadatas[0]["color"].ToString());

        var actualSecondDocument = await chroma.GetDocumentByIdAsync(secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument.PageContent.Should().BeEquivalentTo(texts[1]);
        actualSecondDocument.Metadata["color"].ToString().Should().BeEquivalentTo(metadatas[1]["color"].ToString());
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
    }

    private static string GenerateCollectionName() => "test-" + Guid.NewGuid().ToString("N");

    private Mock<IEmbeddings> CreateFakeEmbeddings()
    {
        var mock = new Mock<IEmbeddings>();

        var embeddingsDict = new Dictionary<string, float[]>
        {
            ["computer"] = new float[] { 1 },
            ["laptop"] = new float[] { 1 },
            ["mainframe"] = new float[] { 1 },
            ["pc"] = new float[] { 1 },
            ["keyboard"] = new float[] { 1 },
            ["mouse"] = new float[] { 1 },
            ["apple"] = new float[] { 1 },
            ["orange"] = new float[] { 1 },
            ["lemon"] = new float[] { 1 },
            ["peach"] = new float[] { 1 },
            ["banana"] = new float[] { 1 },
            ["tomato"] = new float[] { 1 },
            ["tree"] = new float[] { 1 }
        };

        mock.Setup(x => x.EmbedQueryAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>(
                (query, _) =>
                {
                    var embedding = embeddingsDict.TryGetValue(query, out var value)
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
                        embeddings[index] = embeddingsDict.TryGetValue(text, out var value)
                            ? value
                            : throw new ArgumentException("not in dict");
                    }

                    return Task.FromResult(embeddings);
                });

        return mock;
    }
}
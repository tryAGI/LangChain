namespace LangChain.Databases.IntegrationTests;

[TestFixture]
public partial class Tests
{
    [TestCase(SupportedDatabase.InMemory)]
    [TestCase(SupportedDatabase.Chroma, Description = "Requires Chroma server running on localhost:8000. Use `docker run -p 8000:8000 chromadb/chroma`", Explicit = true)]
    [TestCase(SupportedDatabase.SqLite)]
    public async Task SimilaritySearchByVector_Ok(SupportedDatabase database)
    {
        var embeddingsMock = CreateEmbeddingModelMock();
        var vectorStore = GetConfiguredVectorDatabase(database);

        await vectorStore.AddTextsAsync(embeddingsMock.Object, Embeddings.Keys);

        var similar = await vectorStore.SearchAsync(Embeddings["lemon"], new VectorSearchSettings
        {
            NumberOfResults = 5,
        });
        similar.Items.Should().HaveCount(5);

        var similarTexts = similar.Items.Select(s => s.Text).ToArray();

        similarTexts[0].Should().BeEquivalentTo("lemon");
        similarTexts.Should().Contain("orange");
        similarTexts.Should().Contain("peach");
        similarTexts.Should().Contain("banana");
        similarTexts.Should().Contain("apple");
    }
}
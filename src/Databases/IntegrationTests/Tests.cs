using LangChain.Sources;

namespace LangChain.Databases.IntegrationTests;

[TestFixture]
public partial class Tests
{
    [TestCase(SupportedDatabase.Chroma)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)]
    public async Task CreateAndDeleteCollection_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);
        var vectorDatabase = (IVectorDatabaseExtended)environment.VectorDatabase;

        // ReSharper disable once AccessToDisposedClosure
        await vectorDatabase.Invoking(y => y.GetCollectionAsync(environment.CollectionName))
            .Should().ThrowAsync<InvalidOperationException>();
        
        var actual = await vectorDatabase.GetOrCreateCollectionAsync(environment.CollectionName);

        actual.Should().NotBeNull();
        actual.Id.Should().NotBeEmpty();
        actual.Name.Should().BeEquivalentTo(environment.CollectionName);

        await vectorDatabase.DeleteCollectionAsync(environment.CollectionName);
        
        // ReSharper disable once AccessToDisposedClosure
        await vectorDatabase.Invoking(y => y.GetCollectionAsync(environment.CollectionName))
            .Should().ThrowAsync<InvalidOperationException>();
    }
    
    [TestCase(SupportedDatabase.Chroma)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)]
    public async Task AddDocuments_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);
        var vectorDatabase = (IVectorDatabaseExtended)environment.VectorDatabase;

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

        var ids = await vectorDatabase.AddDocumentsAsync(environment.EmbeddingModel, documents);

        ids.Should().HaveCount(2);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        firstId.Should().NotBeEmpty();
        secondId.Should().NotBeEmpty();

        var actualFirstDocument = await vectorDatabase.GetDocumentByIdAsync(environment.CollectionName, firstId);
        actualFirstDocument.Should().NotBeNull();
        actualFirstDocument!.PageContent.Should().BeEquivalentTo(documents[0].PageContent);
        actualFirstDocument.Metadata["color"].Should().BeEquivalentTo(documents[0].Metadata["color"]);

        var actualSecondDocument = await vectorDatabase.GetDocumentByIdAsync(environment.CollectionName, secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument!.PageContent.Should().BeEquivalentTo(documents[1].PageContent);
        actualSecondDocument.Metadata["color"].Should().BeEquivalentTo(documents[1].Metadata["color"]);
    }
    
    [TestCase(SupportedDatabase.Chroma)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)]
    public async Task AddTexts_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);
        var vectorDatabase = (IVectorDatabaseExtended)environment.VectorDatabase;

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

        var ids = await vectorDatabase.AddTextsAsync(environment.EmbeddingModel, texts, metadatas);

        ids.Should().HaveCount(2);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        firstId.Should().NotBeEmpty();
        secondId.Should().NotBeEmpty();

        var actualFirstDocument = await vectorDatabase.GetDocumentByIdAsync(environment.CollectionName, firstId);
        actualFirstDocument.Should().NotBeNull();
        actualFirstDocument!.PageContent.Should().BeEquivalentTo(texts[0]);
        actualFirstDocument.Metadata["string"].Should().BeEquivalentTo(metadatas[0]["string"]);
        actualFirstDocument.Metadata["double"].Should().BeEquivalentTo(metadatas[0]["double"]);
        actualFirstDocument.Metadata["guid"].Should().BeEquivalentTo(metadatas[0]["guid"]);

        var actualSecondDocument = await vectorDatabase.GetDocumentByIdAsync(environment.CollectionName, secondId);
        actualSecondDocument.Should().NotBeNull();
        actualSecondDocument!.PageContent.Should().BeEquivalentTo(texts[1]);
        actualSecondDocument.Metadata["color"].Should().BeEquivalentTo(metadatas[1]["color"]);
    }

    [TestCase(SupportedDatabase.Chroma)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)]
    public async Task DeleteDocuments_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);
        var vectorDatabase = (IVectorDatabaseExtended)environment.VectorDatabase;

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

        var ids = await vectorDatabase.AddDocumentsAsync(environment.EmbeddingModel, documents);

        await vectorDatabase.DeleteAsync(ids);

        var firstId = ids.First();
        var secondId = ids.Skip(1).First();

        var actualFist = await vectorDatabase.GetDocumentByIdAsync(environment.CollectionName, firstId);
        var actualSecond = await vectorDatabase.GetDocumentByIdAsync(environment.CollectionName, secondId);

        actualFist.Should().BeNull();
        actualSecond.Should().BeNull();
    }

    [TestCase(SupportedDatabase.InMemory)]
    [TestCase(SupportedDatabase.Chroma)]
    [TestCase(SupportedDatabase.SqLite)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)]
    //[TestCase(SupportedDatabase.Postgres, Explicit = true)] // #TODO: Fix Postgres tests
    public async Task SimilaritySearch_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);

        await environment.VectorDatabase.AddTextsAsync(environment.EmbeddingModel, Embeddings.Keys);

        var similar = await environment.VectorDatabase.SearchAsync(
            environment.EmbeddingModel,
            embeddingRequest: "lemon",
            searchSettings: new VectorSearchSettings
            {
                Type = VectorSearchType.Similarity,
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

    [TestCase(SupportedDatabase.InMemory)]
    [TestCase(SupportedDatabase.Chroma)]
    [TestCase(SupportedDatabase.SqLite)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)]
    //[TestCase(SupportedDatabase.Postgres, Explicit = true)] // #TODO: Fix Postgres tests
    public async Task SimilaritySearchByVector_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);

        await environment.VectorDatabase.AddTextsAsync(environment.EmbeddingModel, Embeddings.Keys);

        var similar = await environment.VectorDatabase.SearchAsync(Embeddings["lemon"], new VectorSearchSettings
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
    
    //[TestCase(SupportedDatabase.InMemory)]
    //[TestCase(SupportedDatabase.Chroma)]
    //[TestCase(SupportedDatabase.SqLite)]
    [TestCase(SupportedDatabase.OpenSearch, Explicit = true)] // #TODO: Fix OpenSearch tests
    //[TestCase(SupportedDatabase.Postgres, Explicit = true)] // #TODO: Fix Postgres tests
    public async Task SimilaritySearchWithScores_Ok(SupportedDatabase database)
    {
        await using var environment = await StartEnvironmentForAsync(database);

        await environment.VectorDatabase.AddTextsAsync(environment.EmbeddingModel, Embeddings.Keys);

        var similar = await environment.VectorDatabase.SearchAsync(environment.EmbeddingModel, "lemon", searchSettings: new VectorSearchSettings
        {
            NumberOfResults = 5,
        });
        similar.Items.Should().HaveCount(5);

        var first = similar.Items.First();

        first.Text.Should().BeEquivalentTo("lemon");
        if (database is SupportedDatabase.Chroma)
        {
            first.Distance.Should().BeGreaterOrEqualTo(1f);
        }
    }
}
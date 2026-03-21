using System.Text.Json;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.SqliteVec;

namespace LangChain.UnitTest;

[TestFixture]
public class VectorStoreTests
{
    #region LangChainDocumentRecord

    [Test]
    public void DocumentRecord_DefaultId_IsNotEmpty()
    {
        var record = new LangChainDocumentRecord();

        record.Id.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void DocumentRecord_TwoInstances_HaveDifferentIds()
    {
        var record1 = new LangChainDocumentRecord();
        var record2 = new LangChainDocumentRecord();

        record1.Id.Should().NotBe(record2.Id);
    }

    [Test]
    public void DocumentRecord_DefaultValues_AreExpected()
    {
        var record = new LangChainDocumentRecord();

        record.Text.Should().BeNull();
        record.MetadataJson.Should().BeNull();
        record.Embedding.Length.Should().Be(0);
    }

    [Test]
    public void DocumentRecord_SetProperties_RoundTrips()
    {
        var embedding = new float[] { 0.1f, 0.2f, 0.3f };
        var metadata = new Dictionary<string, object> { { "source", "test" }, { "page", 1 } };

        var record = new LangChainDocumentRecord
        {
            Id = "test-id",
            Text = "Hello, world!",
            MetadataJson = JsonSerializer.Serialize(metadata),
            Embedding = new ReadOnlyMemory<float>(embedding),
        };

        record.Id.Should().Be("test-id");
        record.Text.Should().Be("Hello, world!");
        record.Embedding.ToArray().Should().BeEquivalentTo(embedding);

        var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(record.MetadataJson!);
        deserialized.Should().NotBeNull();
        deserialized!["source"].ToString().Should().Be("test");
    }

    #endregion

    #region VectorCollectionExtensions

    [Test]
    public void AsString_EmptyCollection_ReturnsEmptyString()
    {
        var docs = Array.Empty<Document>();

        var result = docs.AsString();

        result.Should().BeEmpty();
    }

    [Test]
    public void AsString_SingleDocument_ReturnsContent()
    {
        var docs = new[] { new Document("Hello") };

        var result = docs.AsString();

        result.Should().Be("Hello");
    }

    [Test]
    public void AsString_MultipleDocuments_JoinsWithSeparator()
    {
        var docs = new[]
        {
            new Document("First"),
            new Document("Second"),
            new Document("Third"),
        };

        var result = docs.AsString();

        result.Should().Be("First\n\nSecond\n\nThird");
    }

    [Test]
    public void AsString_CustomSeparator_Uses()
    {
        var docs = new[]
        {
            new Document("A"),
            new Document("B"),
        };

        var result = docs.AsString(separator: " | ");

        result.Should().Be("A | B");
    }

    [Test]
    public async Task AddAndGet_RoundTrips_WithMetadata()
    {
        var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("test");
        await collection.EnsureCollectionExistsAsync();

        var embedding = new float[] { 1.0f, 0.0f, 0.0f };
        var metadata = new Dictionary<string, object> { { "source", "unit-test" } };

        var record = new LangChainDocumentRecord
        {
            Id = "doc-1",
            Text = "Test document",
            MetadataJson = JsonSerializer.Serialize(metadata),
            Embedding = new ReadOnlyMemory<float>(embedding),
        };

        await collection.UpsertAsync(record);

        var retrieved = await collection.GetAsync("doc-1");

        retrieved.Should().NotBeNull();
        retrieved!.Text.Should().Be("Test document");
        retrieved.Embedding.ToArray().Should().BeEquivalentTo(embedding);

        var deserializedMeta = JsonSerializer.Deserialize<Dictionary<string, object>>(retrieved.MetadataJson!);
        deserializedMeta.Should().NotBeNull();
        deserializedMeta!["source"].ToString().Should().Be("unit-test");
    }

    [Test]
    public async Task GetDocumentByIdAsync_ExistingRecord_ReturnsDocument()
    {
        var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("test");
        await collection.EnsureCollectionExistsAsync();

        var record = new LangChainDocumentRecord
        {
            Id = "doc-2",
            Text = "Some text",
            MetadataJson = null,
            Embedding = new ReadOnlyMemory<float>([0.5f, 0.5f]),
        };

        await collection.UpsertAsync(record);

        var doc = await collection.GetDocumentByIdAsync("doc-2");

        doc.Should().NotBeNull();
        doc!.PageContent.Should().Be("Some text");
    }

    [Test]
    public async Task GetDocumentByIdAsync_MissingRecord_ReturnsNull()
    {
        var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("test");
        await collection.EnsureCollectionExistsAsync();

        var doc = await collection.GetDocumentByIdAsync("nonexistent");

        doc.Should().BeNull();
    }

    [Test]
    public async Task GetDocumentByIdAsync_NullMetadata_ReturnsDocumentWithNullMetadata()
    {
        var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("test");
        await collection.EnsureCollectionExistsAsync();

        var record = new LangChainDocumentRecord
        {
            Id = "doc-null-meta",
            Text = "No metadata",
            MetadataJson = null,
            Embedding = new ReadOnlyMemory<float>([1.0f]),
        };

        await collection.UpsertAsync(record);

        var doc = await collection.GetDocumentByIdAsync("doc-null-meta");

        doc.Should().NotBeNull();
        doc!.PageContent.Should().Be("No metadata");
        doc.Metadata.Should().BeEmpty();
    }

    [Test]
    public async Task GetDocumentByIdAsync_EmptyText_ReturnsEmptyString()
    {
        var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("test");
        await collection.EnsureCollectionExistsAsync();

        var record = new LangChainDocumentRecord
        {
            Id = "doc-empty",
            Text = null,
            MetadataJson = null,
            Embedding = new ReadOnlyMemory<float>([1.0f]),
        };

        await collection.UpsertAsync(record);

        var doc = await collection.GetDocumentByIdAsync("doc-empty");

        doc.Should().NotBeNull();
        doc!.PageContent.Should().Be(string.Empty);
    }

    #endregion

    #region SqliteVec

    /// <summary>
    /// Creates a 1536-dimensional embedding with the given value at the first position (rest zeros).
    /// Matches the [VectorStoreVector(1536)] attribute on LangChainDocumentRecord.
    /// </summary>
    private static ReadOnlyMemory<float> MakeEmbedding(float firstValue, float secondValue = 0f, float thirdValue = 0f)
    {
        var data = new float[1536];
        data[0] = firstValue;
        data[1] = secondValue;
        data[2] = thirdValue;
        return new ReadOnlyMemory<float>(data);
    }

    [Test]
    public async Task SqliteVec_AddAndGet_RoundTrips()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"langchain_test_{Guid.NewGuid():N}.db");
        try
        {
            var store = new SqliteVectorStore($"Data Source={dbPath}");
            var collection = store.GetCollection<string, LangChainDocumentRecord>("test_collection");
            await collection.EnsureCollectionExistsAsync();

            var record = new LangChainDocumentRecord
            {
                Id = "sqlite-doc-1",
                Text = "SQLite test document",
                MetadataJson = JsonSerializer.Serialize(new Dictionary<string, object> { { "key", "value" } }),
                Embedding = MakeEmbedding(1.0f),
            };

            await collection.UpsertAsync(record);

            var retrieved = await collection.GetAsync("sqlite-doc-1");

            retrieved.Should().NotBeNull();
            retrieved!.Text.Should().Be("SQLite test document");
            retrieved.Id.Should().Be("sqlite-doc-1");

            var meta = JsonSerializer.Deserialize<Dictionary<string, object>>(retrieved.MetadataJson!);
            meta.Should().NotBeNull();
            meta!["key"].ToString().Should().Be("value");
        }
        finally
        {
            File.Delete(dbPath);
        }
    }

    [Test]
    public async Task SqliteVec_Search_ReturnsSimilarDocuments()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"langchain_test_{Guid.NewGuid():N}.db");
        try
        {
            var store = new SqliteVectorStore($"Data Source={dbPath}");
            var collection = store.GetCollection<string, LangChainDocumentRecord>("search_test");
            await collection.EnsureCollectionExistsAsync();

            // Add three documents with different embeddings (1536-dim, differ in first 3 positions)
            await collection.UpsertAsync(new LangChainDocumentRecord
            {
                Id = "doc-a",
                Text = "About cats",
                Embedding = MakeEmbedding(1.0f, 0.0f, 0.0f),
            });
            await collection.UpsertAsync(new LangChainDocumentRecord
            {
                Id = "doc-b",
                Text = "About dogs",
                Embedding = MakeEmbedding(0.9f, 0.1f, 0.0f),
            });
            await collection.UpsertAsync(new LangChainDocumentRecord
            {
                Id = "doc-c",
                Text = "About space",
                Embedding = MakeEmbedding(0.0f, 0.0f, 1.0f),
            });

            // Search with embedding similar to doc-a
            var query = MakeEmbedding(1.0f, 0.0f, 0.0f);
            var results = new List<VectorSearchResult<LangChainDocumentRecord>>();
            await foreach (var result in collection.SearchAsync(query, 2))
            {
                results.Add(result);
            }

            results.Should().HaveCount(2);
            // The closest match should be "About cats" (exact match)
            results[0].Record.Text.Should().Be("About cats");
        }
        finally
        {
            File.Delete(dbPath);
        }
    }

    [Test]
    public async Task SqliteVec_EnsureCollectionDeletedAsync_RemovesCollection()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"langchain_test_{Guid.NewGuid():N}.db");
        try
        {
            var store = new SqliteVectorStore($"Data Source={dbPath}");
            var collection = store.GetCollection<string, LangChainDocumentRecord>("delete_test");
            await collection.EnsureCollectionExistsAsync();

            var exists = await collection.CollectionExistsAsync();
            exists.Should().BeTrue();

            await collection.EnsureCollectionDeletedAsync();

            var existsAfter = await collection.CollectionExistsAsync();
            existsAfter.Should().BeFalse();
        }
        finally
        {
            File.Delete(dbPath);
        }
    }

    #endregion

    #region VectorDatabaseExtensions

    [Test]
    public async Task AddDocumentsFromAsync_LoadsAndAddsDocuments()
    {
        // Create a temp text file
        var filePath = Path.Combine(Path.GetTempPath(), $"langchain_test_{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(filePath, "The quick brown fox jumps over the lazy dog.");

            var store = new InMemoryVectorStore();
            var embeddingGenerator = new FakeEmbeddingGenerator();

            var collection = await store.AddDocumentsFromAsync<FileLoader>(
                embeddingGenerator,
                dimensions: 3,
                dataSource: DataSource.FromPath(filePath),
                collectionName: "test-add-docs");

            collection.Should().NotBeNull();

            // The FileLoader returns one document. The CharacterTextSplitter (default) may split it,
            // but with this short text it should stay as one chunk.
            // Verify we can search and find the document
            var results = new List<VectorSearchResult<LangChainDocumentRecord>>();
            var queryEmbedding = new ReadOnlyMemory<float>([1.0f, 0.0f, 0.0f]);
            await foreach (var result in collection.SearchAsync(queryEmbedding, 1))
            {
                results.Add(result);
            }

            results.Should().HaveCountGreaterThanOrEqualTo(1);
            results[0].Record.Text.Should().Contain("fox");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Test]
    public async Task AddDocumentsFromAsync_JustReturnCollection_WhenAlreadyExists()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"langchain_test_{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(filePath, "First load content.");

            var store = new InMemoryVectorStore();
            var embeddingGenerator = new FakeEmbeddingGenerator();

            // First call creates collection and adds docs
            var collection1 = await store.AddDocumentsFromAsync<FileLoader>(
                embeddingGenerator,
                dimensions: 3,
                dataSource: DataSource.FromPath(filePath),
                collectionName: "skip-test",
                behavior: AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists);

            // Overwrite file with different content
            await File.WriteAllTextAsync(filePath, "Second load content - should be ignored.");

            // Second call should just return existing collection without adding
            var collection2 = await store.AddDocumentsFromAsync<FileLoader>(
                embeddingGenerator,
                dimensions: 3,
                dataSource: DataSource.FromPath(filePath),
                collectionName: "skip-test",
                behavior: AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists);

            // Search should still find the original content
            var results = new List<VectorSearchResult<LangChainDocumentRecord>>();
            var queryEmbedding = new ReadOnlyMemory<float>([1.0f, 0.0f, 0.0f]);
            await foreach (var result in collection2.SearchAsync(queryEmbedding, 10))
            {
                results.Add(result);
            }

            results.Should().HaveCountGreaterThanOrEqualTo(1);
            results[0].Record.Text.Should().Contain("First load");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// A fake embedding generator that returns deterministic 3-dimensional embeddings for testing.
    /// </summary>
    private sealed class FakeEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
    {
        public EmbeddingGeneratorMetadata Metadata { get; } = new("fake");

        public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
            IEnumerable<string> values,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var embeddings = new GeneratedEmbeddings<Embedding<float>>(
                values.Select(v =>
                {
                    // Simple deterministic embedding based on string hash
                    var hash = v.GetHashCode();
                    var embedding = new float[]
                    {
                        (hash & 0xFF) / 255f,
                        ((hash >> 8) & 0xFF) / 255f,
                        ((hash >> 16) & 0xFF) / 255f,
                    };
                    return new Embedding<float>(embedding);
                }).ToList());

            return Task.FromResult(embeddings);
        }

        public object? GetService(Type serviceType, object? serviceKey = null) => null;

        public void Dispose() { }
    }

    #endregion
}

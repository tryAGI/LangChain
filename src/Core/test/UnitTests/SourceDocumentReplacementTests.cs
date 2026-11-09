using System.Text.Json;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace LangChain.UnitTest;

[TestFixture]
public class SourceDocumentReplacementTests
{
    private const string SourceA = "folder/first file.pdf";
    private const string SourceB = "folder/other file.pdf";

    [Test]
    public async Task TypedReplacement_RemovesOnlyPreviousSourceChunks()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("typed-source-replace");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator,
            [DocumentFor(SourceA, "A1"), DocumentFor(SourceA, "A2")]);
        var oldB = await collection.AddDocumentsAsync(generator,
            [DocumentFor(SourceB, "B1")]);

        var newA = await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, oldA.Concat(oldA).ToArray(),
            [DocumentFor(SourceA, "A new 1"), DocumentFor(SourceA, "A new 2")]);

        newA.Should().HaveCount(2);
        foreach (var id in oldA)
        {
            (await collection.GetAsync(id)).Should().BeNull();
        }

        var other = await collection.GetDocumentByIdAsync(oldB.Single());
        other.Should().NotBeNull();
        other!.PageContent.Should().Be("B1");
        foreach (var id in newA)
        {
            (await collection.GetDocumentByIdAsync(id))!.Metadata["path"]
                .ToString().Should().Be(SourceA);
        }
    }

    [Test]
    public async Task DynamicReplacement_RemovesOnlyPreviousSourceChunks()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetDynamicCollection(
            "dynamic-source-replace",
            new VectorStoreCollectionDefinition
            {
                Properties =
                [
                    new VectorStoreKeyProperty("Id", typeof(string)),
                    new VectorStoreDataProperty("Text", typeof(string)),
                    new VectorStoreDataProperty("MetadataJson", typeof(string)),
                    new VectorStoreVectorProperty("Embedding", 3) { Type = typeof(ReadOnlyMemory<float>) },
                ]
            });
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator,
            [DocumentFor(SourceA, "A1"), DocumentFor(SourceA, "A2")]);
        var oldB = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceB, "B1")]);

        var newA = await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, oldA, [DocumentFor(SourceA, "A updated")]);

        newA.Should().ContainSingle();
        foreach (var id in oldA)
        {
            (await collection.GetDocumentByIdAsync(id)).Should().BeNull();
        }

        (await collection.GetDocumentByIdAsync(oldB.Single()))!.PageContent.Should().Be("B1");
        (await collection.GetDocumentByIdAsync(newA.Single()))!.PageContent.Should().Be("A updated");
    }

    [Test]
    public async Task TypedReplacement_RejectsIdsFromAnotherSourceWithoutMutation()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("reject-other-source");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceA, "A1")]);
        var oldB = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceB, "B1")]);

        var action = async () => await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, oldA.Concat(oldB).ToArray(), [DocumentFor(SourceA, "A new")]);

        await action.Should().ThrowAsync<InvalidOperationException>();
        (await collection.GetAsync(oldA.Single())).Should().NotBeNull();
        (await collection.GetAsync(oldB.Single())).Should().NotBeNull();
    }

    [Test]
    public async Task TypedReplacement_RejectsUnrelatedNewSourceWithoutMutation()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("reject-new-source");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceA, "A1")]);
        var action = async () => await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, oldA, [DocumentFor(SourceB, "B unexpectedly")]);

        await action.Should().ThrowAsync<InvalidOperationException>();
        (await collection.GetAsync(oldA.Single())).Should().NotBeNull();
    }

    [Test]
    public async Task TypedReplacement_RejectsMissingPreviousIdWithoutMutation()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("reject-missing-id");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceA, "A1")]);
        var action = async () => await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, ["nonexistent"], [DocumentFor(SourceA, "New A")]);

        await action.Should().ThrowAsync<InvalidOperationException>();
        (await collection.GetAsync(oldA.Single())).Should().NotBeNull();
    }

    [Test]
    public async Task TypedReplacement_CancellationKeepsOriginalRecords()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("cancel-source-replace");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceA, "A1")]);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var action = async () => await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, oldA, [DocumentFor(SourceA, "New A")], cts.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
        (await collection.GetAsync(oldA.Single())).Should().NotBeNull();
    }

    [Test]
    public async Task TypedReplacement_FailedEmbeddingsKeepOldRecords()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("embedding-failure");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();
        using var failedGenerator = new FailingEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceA, "A1")]);
        var action = async () => await collection.ReplaceSourceDocumentsAsync(
            failedGenerator, SourceA, oldA, [DocumentFor(SourceA, "New A")]);

        await action.Should().ThrowAsync<InvalidOperationException>();
        (await collection.GetAsync(oldA.Single())).Should().NotBeNull();
    }

    [Test]
    public async Task TypedReplacement_RejectsEmptyDocumentsWithoutMutation()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("reject-empty");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();

        var oldA = await collection.AddDocumentsAsync(generator, [DocumentFor(SourceA, "A1")]);
        var action = async () => await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, oldA, Array.Empty<Document>());

        await action.Should().ThrowAsync<ArgumentException>();
        (await collection.GetAsync(oldA.Single())).Should().NotBeNull();
    }

    [Test]
    public async Task TypedReplacementFromFileLoader_UsesPathMetadata()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"replace_{Guid.NewGuid():N}.txt");
        try
        {
            await File.WriteAllTextAsync(filePath, "First version of a file.");
            using var store = new InMemoryVectorStore();
            var collection = store.GetCollection<string, LangChainDocumentRecord>("typed-file-replace");
            await collection.EnsureCollectionExistsAsync();
            using var generator = new TestEmbeddingGenerator();

            var originalIds = await collection.AddDocumentsFromAsync<FileLoader>(
                generator, DataSource.FromPath(filePath),
                loaderSettings: new DocumentLoaderSettings());

            originalIds.Should().NotBeEmpty();
            await File.WriteAllTextAsync(filePath, "Updated version of the same file.");
            var newIds = await collection.ReplaceSourceDocumentsFromAsync<FileLoader>(
                generator, filePath, originalIds, DataSource.FromPath(filePath));

            newIds.Should().NotBeEmpty();
            foreach (var id in originalIds)
            {
                (await collection.GetAsync(id)).Should().BeNull();
            }

            var updated = await collection.GetDocumentByIdAsync(newIds.First());
            updated!.PageContent.Should().Contain("Updated version");
            updated.Metadata["path"].ToString().Should().Be(filePath);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Test]
    public async Task TypedReplacement_AllowsLegacySourceMetadata()
    {
        using var store = new InMemoryVectorStore();
        var collection = store.GetCollection<string, LangChainDocumentRecord>("legacy-source-key");
        await collection.EnsureCollectionExistsAsync();
        using var generator = new TestEmbeddingGenerator();
        var old = await collection.AddDocumentsAsync(generator,
            [new Document("Before", new Dictionary<string, object> { ["source"] = SourceA })]);

        var replacement = await collection.ReplaceSourceDocumentsAsync(
            generator, SourceA, old,
            [new Document("After", new Dictionary<string, object> { ["source"] = SourceA })]);

        replacement.Should().ContainSingle();
        (await collection.GetDocumentByIdAsync(old.Single())).Should().BeNull();
        (await collection.GetDocumentByIdAsync(replacement.Single()))!.PageContent.Should().Be("After");
    }

    private static Document DocumentFor(string source, string content)
        => new(content, new Dictionary<string, object> { ["path"] = source });

    private sealed class TestEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
    {
        public EmbeddingGeneratorMetadata Metadata { get; } = new("test");

        public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
            IEnumerable<string> values,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new GeneratedEmbeddings<Embedding<float>>(
                values.Select(_ => new Embedding<float>(new float[] { 1, 0, 0 })).ToList()));
        }

        public object? GetService(Type serviceType, object? serviceKey = null) => null;
        public void Dispose() { }
    }

    private sealed class FailingEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
    {
        public EmbeddingGeneratorMetadata Metadata { get; } = new("failing");

        public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
            IEnumerable<string> values,
            EmbeddingGenerationOptions? options = null,
            CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Embedding service unavailable");

        public object? GetService(Type serviceType, object? serviceKey = null) => null;
        public void Dispose() { }
    }
}

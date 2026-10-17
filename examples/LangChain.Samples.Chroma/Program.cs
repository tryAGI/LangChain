using System.ClientModel;
using System.Text.Json;
using ChromaDB.Client;
using CommunityToolkit.VectorData.Chroma;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using OpenAI;

if (args.Contains("--nativeaot-smoke", StringComparer.Ordinal))
{
    await RunNativeAotSmokeAsync();
    return;
}

// Connect to Ollama via its OpenAI-compatible endpoint
// (llmman, https://github.com/llmmanorg/llmman, serves the same API on port 17434)
var ollamaEndpoint = new Uri("http://localhost:11434/v1");
var ollamaOptions = new OpenAIClientOptions { Endpoint = ollamaEndpoint };
var ollamaApiKey = new ApiKeyCredential("ollama");

IChatClient llm = new OpenAIClient(ollamaApiKey, ollamaOptions)
    .GetChatClient("llama3").AsIChatClient();
IEmbeddingGenerator<string, Embedding<float>> embeddingModel = new OpenAIClient(ollamaApiKey, ollamaOptions)
    .GetEmbeddingClient("all-minilm").AsIEmbeddingGenerator();

// Chroma, started with: docker run -p 8000:8000 chromadb/chroma
using var vectorStore = new ChromaVectorStore(new ChromaClient("http://localhost:8000"), ownsClient: true);

var vectorCollection = await vectorStore.AddDocumentsFromDynamicAsync<PdfPigPdfLoader>(
    embeddingModel, // Used to convert text to embeddings
    dimensions: 384, // Should be 384 for all-minilm
    dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
    collectionName: "harrypotter", // Can be omitted, use if you want to have multiple collections
    textSplitter: null,
    behavior: AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists);


const string question = "What is Harry's Address?";
var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, question, amount: 5);
// Use similar documents and LLM to answer the question
var response = await llm.GetResponseAsync(
    $"""
     Use the following pieces of context to answer the question at the end.
     If the answer is not in context then just say that you don't know, don't try to make up an answer.
     Keep the answer as short as possible.

     {similarDocuments.AsString()}

     Question: {question}
     Helpful Answer:
     """);

Console.WriteLine($"LLM answer: {response.Text}");

static async Task RunNativeAotSmokeAsync()
{
    var chromaUrl = Environment.GetEnvironmentVariable("CHROMA_URL") ?? "http://localhost:8000";
    using var vectorStore = new ChromaVectorStore(new ChromaClient(chromaUrl), ownsClient: true);

    var collectionName = $"langchain-aot-{Guid.NewGuid():N}";
    var filePath = Path.Combine(Path.GetTempPath(), $"{collectionName}.txt");
    await File.WriteAllTextAsync(filePath, "NativeAOT vector mapping smoke test.");

    VectorStoreCollection<object, Dictionary<string, object?>>? collection = null;
    try
    {
        var embeddingGenerator = new SmokeTestEmbeddingGenerator();
        collection = await vectorStore.AddDocumentsFromDynamicAsync<FileLoader>(
            embeddingGenerator,
            dimensions: 3,
            dataSource: DataSource.FromPath(filePath),
            collectionName: collectionName,
            behavior: AddDocumentsToDatabaseBehavior.OverwriteExistingCollection);

        const string text = "AOT metadata survives Chroma round trips.";
        var metadata = new Dictionary<string, object>
        {
            ["source"] = "nativeaot-smoke",
            ["page"] = 3,
        };
        var ids = await collection.AddTextsAsync(embeddingGenerator, [text], [metadata]);
        var document = await collection.GetDocumentByIdAsync(ids.Single());

        if (document?.PageContent != text ||
            document.Metadata["source"] is not JsonElement { ValueKind: JsonValueKind.String } source ||
            source.GetString() != "nativeaot-smoke" ||
            document.Metadata["page"] is not JsonElement { ValueKind: JsonValueKind.Number } page ||
            page.GetInt32() != 3)
        {
            throw new InvalidOperationException("Chroma did not preserve the native AOT document and metadata round trip.");
        }

        var similarDocuments = await collection.GetSimilarDocuments(embeddingGenerator, text, amount: 2);
        if (!similarDocuments.Any(result => result.PageContent == text))
        {
            throw new InvalidOperationException("Chroma similarity search did not return the inserted native AOT document.");
        }

        Console.WriteLine("NativeAOT Chroma upsert, read, metadata, and search round trip passed.");
    }
    finally
    {
        if (collection is not null)
        {
            await collection.EnsureCollectionDeletedAsync();
        }

        File.Delete(filePath);
    }
}

sealed class SmokeTestEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    public EmbeddingGeneratorMetadata Metadata { get; } = new("nativeaot-smoke");

    public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var embeddings = new GeneratedEmbeddings<Embedding<float>>(
            values.Select(_ => new Embedding<float>(new float[] { 1f, 0f, 0f })).ToList());
        return Task.FromResult(embeddings);
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose() { }
}

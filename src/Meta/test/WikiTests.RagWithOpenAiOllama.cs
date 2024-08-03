using LangChain.Databases.Sqlite;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.DocumentLoaders;
using LangChain.Providers.Ollama;
using LangChain.Splitters.Text;
using Ollama;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task RagWithOpenAiOllama()
    {
        // prepare OpenAI embedding model
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OpenAI API key is not set");
        var embeddingModel = new TextEmbeddingV3SmallModel(apiKey);

        // prepare Ollama with mistral model
        var provider = new OllamaProvider(
            options: new RequestOptions
            {
                Stop = ["\n"],
                Temperature = 0.0f,
            });
        var llm = new OllamaChatModel(provider, id: "mistral:latest").UseConsoleForDebug();

        using var vectorDatabase = new SqLiteVectorDatabase("vectors.db");
        var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
            embeddingModel,
            dimensions: 1536, // Should be 1536 for TextEmbeddingV3SmallModel
            dataSource: DataSource.FromPath("E:\\AI\\Datasets\\Books\\Harry-Potter-Book-1.pdf"),
            collectionName: "harrypotter",
            textSplitter: new RecursiveCharacterTextSplitter(
                chunkSize: 200,
                chunkOverlap: 50));

        string promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";


        var chain =
            Set("Who was drinking a unicorn blood?", outputKey: "question")                // set the question
            | RetrieveDocuments(vectorCollection, embeddingModel, inputKey: "question", outputKey: "documents", amount: 5) // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")                       // combine documents together and put them into context
            | Template(promptText)                                                              // replace context and question in the prompt with their values
            | LLM(llm);                                                                       // send the result to the language model

        var result = await chain.RunAsync("text", CancellationToken.None);                                        // get chain result

        Console.WriteLine(result);
    }
}
using LangChain.Extensions;
using LangChain.DocumentLoaders;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.SqliteVec;
using OpenAI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class ReadmeTests
{
    [Test]
    public async Task Chains1()
    {
        var (chatClient, embeddingGenerator) = Helpers.GetModels(ProviderType.OpenAi);

        var store = new InMemoryVectorStore();
        var vectorCollection = store.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        await vectorCollection.AddDocumentsAsync(embeddingGenerator, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        var chain = (
            Set("What is the good name for a pet?", outputKey: "question") |
            RetrieveDocuments(vectorCollection, embeddingGenerator, inputKey: "question", outputKey: "documents") |
            StuffDocuments(inputKey: "documents", outputKey: "context") |
            Template("""
                Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.

                {context}

                Question: {question}
                Helpful Answer:
                """, outputKey: "prompt") |
            LLM(chatClient, inputKey: "prompt", outputKey: "pet_sentence")) >>
            Template("""
                Human will provide you with sentence about pet. You need to answer with pet name.

                Human: My dog name is Jack
                Answer: Jack
                Human: I think the best name for a pet is "Jerry"
                Answer: Jerry
                Human: {pet_sentence}
                Answer:
                """, outputKey: "prompt") |
            LLM(chatClient, inputKey: "prompt", outputKey: "text");

        var result = await chain.RunAsync(resultKey: "text");
        result.Should().Be("Bob");
    }

    /// <summary>
    /// Price to run from zero(create embeddings and request to LLM): 0,015$
    /// Price to re-run if database is exists: 0,0004$
    /// </summary>
    /// <exception cref="InconclusiveException"></exception>
    [Test]
    public async Task Readme()
    {
        // Initialize models
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set");
        var openAiClient = new OpenAIClient(apiKey);
        IChatClient chatClient = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

        // Create vector database from Harry Potter book pdf
        var vectorStore = new SqliteVectorStore("Data Source=vectors.db");
        var vectorCollection = await vectorStore.AddDocumentsFromAsync<PdfPigPdfLoader>(
            embeddingGenerator, // Used to convert text to embeddings
            dimensions: 1536, // Should be 1536 for text-embedding-3-small
            dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
            collectionName: "harrypotter", // Can be omitted, use if you want to have multiple collections
            textSplitter: null); // Default is CharacterTextSplitter(ChunkSize = 4000, ChunkOverlap = 200)

        // Now we have two ways: use the async methods or use the chains
        // 1. Async methods

        // Find similar documents for the question
        const string question = "Who was drinking a unicorn blood?";
        var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingGenerator, question, amount: 5);

        // Use similar documents and LLM to answer the question
        var answer = await chatClient.GetResponseAsync(
            $"""
             Use the following pieces of context to answer the question at the end.
             If the answer is not in context then just say that you don't know, don't try to make up an answer.
             Keep the answer as short as possible.

             {similarDocuments.AsString()}

             Question: {question}
             Helpful Answer:
             """);

        Console.WriteLine($"LLM answer: {answer.Text}"); // The cloaked figure.

        // 2. Chains
        var promptTemplate =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible. Always quote the context in your answer.
{context}
Question: {text}
Helpful Answer:";

        var chain =
            Set("Who was drinking a unicorn blood?")     // set the question (default key is "text")
            | RetrieveSimilarDocuments(vectorCollection, embeddingGenerator, amount: 5) // take 5 most similar documents
            | CombineDocuments(outputKey: "context")     // combine documents together and put them into context
            | Template(promptTemplate)                   // replace context and question in the prompt with their values
            | LLM(chatClient);                           // send the result to the language model
        var chainAnswer = await chain.RunAsync("text");  // get chain result

        Console.WriteLine("Chain Answer:" + chainAnswer); // print the result
    }

    [Test]
    public async Task SimpleTestUsingAsync()
    {
        var (chatClient, embeddingGenerator) = Helpers.GetModels(ProviderType.OpenAi);

        var store = new InMemoryVectorStore();
        var vectorCollection = store.GetCollection<string, LangChainDocumentRecord>("default");
        await vectorCollection.EnsureCollectionExistsAsync();

        await vectorCollection.AddDocumentsAsync(embeddingGenerator, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        const string question = "What is the good name for a pet?";
        var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingGenerator, question, amount: 1);

        Console.WriteLine($"Similar Documents: {similarDocuments.AsString()}");

        var petNameResponse = await chatClient.GetResponseAsync(
            $"""

             Human will provide you with sentence about pet. You need to answer with pet name.

             Human: My dog name is Jack
             Answer: Jack
             Human: I think the best name for a pet is "Jerry"
             Answer: Jerry
             Human: {similarDocuments.AsString()}
             Answer:
             """);

        Console.WriteLine($"LLM answer: {petNameResponse.Text}");

        petNameResponse.Text.Should().Be("Bob");
    }
}

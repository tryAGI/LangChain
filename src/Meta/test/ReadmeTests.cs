using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Databases.Sqlite;
using LangChain.Extensions;
using LangChain.Sources;
using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class ReadmeTests
{
    [Test]
    public async Task Chains1()
    {
        var (llm, embeddings) = Helpers.GetModels(ProviderType.OpenAi);
        var vectorCollection = new InMemoryVectorCollection();
        await vectorCollection.AddDocumentsAsync(embeddings, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        var chain = (
            Set("What is the good name for a pet?", outputKey: "question") |
            RetrieveDocuments(vectorCollection, embeddings, inputKey: "question", outputKey: "documents") |
            StuffDocuments(inputKey: "documents", outputKey: "context") |
            Template("""
                Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.

                {context}

                Question: {question}
                Helpful Answer:
                """, outputKey: "prompt") |
            LLM(llm, inputKey: "prompt", outputKey: "pet_sentence")) >>
            Template("""
                Human will provide you with sentence about pet. You need to answer with pet name.

                Human: My dog name is Jack
                Answer: Jack
                Human: I think the best name for a pet is "Jerry"
                Answer: Jerry
                Human: {pet_sentence}
                Answer:
                """, outputKey: "prompt") |
            LLM(llm, inputKey: "prompt", outputKey: "text");

        var result = await chain.RunAsync(resultKey: "text", CancellationToken.None);
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
        var provider = new OpenAiProvider(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        var llm = new Gpt35TurboModel(provider);
        var embeddingModel = new TextEmbeddingV3SmallModel(provider);

        // Create vector database from Harry Potter book pdf
        var vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");
        var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync(collectionName: "harrypotter", dimensions: 1536); // Should be 1536 for TextEmbeddingV3SmallModel

        using (var source = await PdfPigPdfSource.CreateFromUriAsync(
            new Uri("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf")))
        {
            await vectorCollection.LoadAndSplitDocuments(
                embeddingModel,
                sources: new[] { source }).ConfigureAwait(false);
        }

        // Now we have two ways: use the async methods or use the chains
        // 1. Async methods

        // Find similar documents for the question
        const string question = "Who was drinking a unicorn blood?";
        var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, question, amount: 5);

        // Use similar documents and LLM to answer the question
        var answer = await llm.GenerateAsync(
            $"""
             Use the following pieces of context to answer the question at the end.
             If the answer is not in context then just say that you don't know, don't try to make up an answer.
             Keep the answer as short as possible.

             {similarDocuments.AsString()}

             Question: {question}
             Helpful Answer:
             """, cancellationToken: CancellationToken.None).ConfigureAwait(false);

        Console.WriteLine($"LLM answer: {answer}"); // The cloaked figure.

        // 2. Chains
        var promptTemplate =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible. Always quote the context in your answer.
{context}
Question: {text}
Helpful Answer:";

        var chain =
            Set("Who was drinking a unicorn blood?")     // set the question (default key is "text")
            | RetrieveSimilarDocuments(vectorCollection, embeddingModel, amount: 5) // take 5 most similar documents
            | CombineDocuments(outputKey: "context")     // combine documents together and put them into context
            | Template(promptTemplate)                   // replace context and question in the prompt with their values
            | LLM(llm.UseConsoleForDebug());             // send the result to the language model
        var chainAnswer = await chain.RunAsync("text", CancellationToken.None);  // get chain result

        Console.WriteLine("Chain Answer:" + chainAnswer); // print the result

        Console.WriteLine($"LLM usage: {llm.Usage}");    // Print usage and price
        Console.WriteLine($"Embedding model usage: {embeddingModel.Usage}");   // Print usage and price
    }

    [Test]
    public async Task SimpleTestUsingAsync()
    {
        var (llm, embeddings) = Helpers.GetModels(ProviderType.OpenAi);

        var vectorCollection = new InMemoryVectorCollection();
        await vectorCollection.AddDocumentsAsync(embeddings, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        const string question = "What is the good name for a pet?";
        var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddings, question, amount: 1);

        Console.WriteLine($"Similar Documents: {similarDocuments.AsString()}");

        var petNameResponse = await llm.GenerateAsync(
            $"""

             Human will provide you with sentence about pet. You need to answer with pet name.

             Human: My dog name is Jack
             Answer: Jack
             Human: I think the best name for a pet is "Jerry"
             Answer: Jerry
             Human: {similarDocuments.AsString()}
             Answer:
             """, cancellationToken: CancellationToken.None).ConfigureAwait(false);

        Console.WriteLine($"LLM answer: {petNameResponse}");
        Console.WriteLine($"Total usage: {llm.Usage}");

        petNameResponse.ToString().Should().Be("Bob");
    }
}
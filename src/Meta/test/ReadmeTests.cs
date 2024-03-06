using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Sources;
using LangChain.Indexes;
using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.Splitters.Text;
using LangChain.VectorStores;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public class ReadmeTests
{
    [Explicit]
    [Test]
    public async Task Chains1()
    {
        var gpt4 = new Gpt4Model(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        var embeddings = new TextEmbeddingV3SmallModel(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        var index = await InMemoryVectorStore.CreateIndexFromDocuments(embeddings, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        var chain = (
            Set("What is the good name for a pet?", outputKey: "question") |
            RetrieveDocuments(index, inputKey: "question", outputKey: "documents") |
            StuffDocuments(inputKey: "documents", outputKey: "context") |
            Template("""
                Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.

                {context}

                Question: {question}
                Helpful Answer:
                """, outputKey: "prompt") |
            LLM(gpt4, inputKey: "prompt", outputKey: "pet_sentence")) >>
            Template("""
                Human will provide you with sentence about pet. You need to answer with pet name.

                Human: My dog name is Jack
                Answer: Jack
                Human: I think the best name for a pet is "Jerry"
                Answer: Jerry
                Human: {pet_sentence}
                Answer:
                """, outputKey: "prompt") |
            LLM(gpt4, inputKey: "prompt", outputKey: "text");

        var result = await chain.Run(resultKey: "text");
        result.Should().Be("Bob");
    }
    
    /// <summary>
    /// Price to run from zero(create embedding and request to LLM): 0,015$
    /// Price to re-run if database is exists: 0,0004$
    /// </summary>
    /// <exception cref="InconclusiveException"></exception>
    [Explicit]
    [Test]
    public async Task RagWithOpenAiUsingChains()
    {
        var gpt35 = new Gpt35TurboModel(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        var embeddings = new TextEmbeddingV3SmallModel(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        
        // Create database with embeddings from Harry Potter book pdf
        if (!File.Exists("vectors.db"))
        {
            await using var stream = H.Resources.HarryPotterBook1_pdf.AsStream();
            var documents = await PdfPigPdfSource.FromStreamAsync(stream);
            
            await SQLiteVectorStore.CreateIndexFromDocuments(
                embeddings: embeddings,
                documents: documents,
                filename: "vectors.db",
                tableName: "vectors",
                textSplitter: new RecursiveCharacterTextSplitter(
                    chunkSize: 200,
                    chunkOverlap: 50));
        }

        var vectorStore = new SQLiteVectorStore("vectors.db", "vectors", embeddings);
        var index = new VectorStoreIndexWrapper(vectorStore);
        var chain =
            // set the question
            Set("Who was drinking a unicorn blood?", outputKey: "question") |
            // take 5 most similar documents
            RetrieveSimilarDocuments(index, inputKey: "question", outputKey: "documents", amount: 5) |
            // combine documents together and put them into context
            CombineDocuments(inputKey: "documents", outputKey: "context") |
            // replace context and question in the prompt with their values
            Template(@"Use the following pieces of context to answer the question at the end.
If the answer is not in context then just say that you don't know, don't try to make up an answer.
Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:") |
            // send the result to the language model
            LargeLanguageModel(gpt35);

        var result = await chain.Run("text");
        
        Console.WriteLine($"LLM answer: {result}");
        Console.WriteLine($"Total usage: {gpt35.Usage}");
    }
    
    /// <summary>
    /// Price to run from zero(create embeddings and request to LLM): 0,015$
    /// Price to re-run if database is exists: 0,0004$
    /// </summary>
    /// <exception cref="InconclusiveException"></exception>
    [Explicit]
    [Test]
    public async Task Readme()
    {
        // Initialize models
        var provider = new OpenAiProvider(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        var llm = new Gpt35TurboModel(provider);
        var embeddings = new TextEmbeddingV3SmallModel(provider);
        
        // Create vector database from Harry Potter book pdf
        var source = await PdfPigPdfSource.CreateFromUriAsync(new Uri("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"));
        var index = await SQLiteVectorStore.GetOrCreateIndexAsync(embeddings, source);
        
        // Now we have two ways: use the async methods or use the chains
        // 1. Async methods
        
        // Find similar documents for the question
        const string question = "Who was drinking a unicorn blood?";
        var similarDocuments = await index.Store.GetSimilarDocuments(question, amount: 5);
        
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
        Console.WriteLine($"LLM usage: {llm.Usage}");
        Console.WriteLine($"Embeddings usage: {embeddings.Usage}");
        
        // 2. Chains
        var promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible. Always quote the context in your answer.
{context}
Question: {text}
Helpful Answer:";

        var chain =
            Set("Who was drinking a unicorn blood?")    // set the question
            | RetrieveDocuments(index, amount: 5)       // take 5 most similar documents
            | StuffDocuments(outputKey: "context")      // combine documents together and put them into context
            | Template(promptText)                      // replace context and question in the prompt with their values
            | LLM(llm.UseConsoleForDebug());            // send the result to the language model
        var chainAnswer = await chain.Run("text");         // get chain result
        
        Console.WriteLine("Answer:"+ chainAnswer);       // print the result
    }

    [Explicit]
    [Test]
    public async Task SimpleDocuments()
    {
        // https://www.together.ai/blog/embeddings-endpoint-release
        // var together = new OpenAiModel(new OpenAiConfiguration
        // {
        //     ApiKey = Environment.GetEnvironmentVariable("TOGETHER_API_KEY") ??
        //              throw new InconclusiveException("TOGETHER_API_KEY is not set"),
        //     Endpoint = "api.together.xyz",
        //     ModelId = "togethercomputer/Qwen-7B",
        //     EmbeddingModelId = "togethercomputer/m2-bert-80M-32k-retrieval",
        // });
        var gpt35 = new Gpt35TurboModel(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        var embeddings = new TextEmbeddingV3SmallModel(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        
        var database = await InMemoryVectorStore.CreateIndexFromDocuments(embeddings, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        const string question = "What is the good name for a pet?";
        var similarDocuments = await database.Store.GetSimilarDocuments(question, amount: 1);
        
        var petNameResponse = await gpt35.GenerateAsync(
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
        Console.WriteLine($"Total usage: {gpt35.Usage}");
        
        petNameResponse.ToString().Should().Be("Bob");
    }
}
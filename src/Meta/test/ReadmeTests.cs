using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Sources;
using LangChain.Providers;
using LangChain.Providers.Anyscale;
using LangChain.Providers.Anyscale.Predefined;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.VectorStores;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class ReadmeTests
{
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
        
        // 2. Chains
        var promptTemplate =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible. Always quote the context in your answer.
{context}
Question: {text}
Helpful Answer:";

        var chain =
            Set("Who was drinking a unicorn blood?")     // set the question (default key is "text")
            | RetrieveSimilarDocuments(index, amount: 5) // take 5 most similar documents
            | CombineDocuments(outputKey: "context")     // combine documents together and put them into context
            | Template(promptTemplate)                   // replace context and question in the prompt with their values
            | LLM(llm.UseConsoleForDebug());             // send the result to the language model
        var chainAnswer = await chain.Run("text");  // get chain result
        
        Console.WriteLine("Chain Answer:"+ chainAnswer); // print the result
        
        Console.WriteLine($"LLM usage: {llm.Usage}");    // Print usage and price
        Console.WriteLine($"Embeddings usage: {embeddings.Usage}");   // Print usage and price
    }

    private enum ProviderType
    {
        OpenAi,
        Together,
        Anyscale,
    }
    
    private (IChatModel ChatModel, IEmbeddingModel EmbeddingModel) GetModels(ProviderType providerType)
    {
        switch (providerType)
        {
            case ProviderType.OpenAi:
            {
                var provider = new OpenAiProvider(
                    Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                    throw new InconclusiveException("OPENAI_API_KEY is not set"));
                var llm = new Gpt35TurboModel(provider);
                var embeddings = new TextEmbeddingV3SmallModel(provider);
                
                return (llm, embeddings);
            }
            case ProviderType.Together:
            {
                // https://www.together.ai/blog/embeddings-endpoint-release
                var provider = new OpenAiProvider(
                    apiKey: Environment.GetEnvironmentVariable("TOGETHER_API_KEY") ??
                    throw new InconclusiveException("TOGETHER_API_KEY is not set"),
                    customEndpoint: "api.together.xyz");
                var llm = new OpenAiChatModel(provider, id: "meta-llama/Llama-2-70b-chat-hf");
                var embeddings = new OpenAiEmbeddingModel(provider, id: "togethercomputer/m2-bert-80M-2k-retrieval");
                
                return (llm, embeddings);
            }
            case ProviderType.Anyscale:
            {
                // https://app.endpoints.anyscale.com/
                var provider = new AnyscaleProvider(
                    apiKey: Environment.GetEnvironmentVariable("ANYSCALE_API_KEY") ??
                            throw new InconclusiveException("ANYSCALE_API_KEY is not set"));
                var llm = new Llama2LargeModel(provider);
                
                // Use OpenAI embeddings for now because Anyscale doesn't have embeddings yet
                var embeddings = new TextEmbeddingV3SmallModel(
                    Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                    throw new InconclusiveException("OPENAI_API_KEY is not set"));
                
                return (llm, embeddings);
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    [Test]
    public async Task SimpleTestUsingAsync()
    {
        var (llm, embeddings) = GetModels(ProviderType.OpenAi);
        
        var database = await InMemoryVectorStore.CreateIndexFromDocuments(embeddings, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        const string question = "What is the good name for a pet?";
        var similarDocuments = await database.Store.GetSimilarDocuments(question, amount: 1);
        
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
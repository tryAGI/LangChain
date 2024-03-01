using System.Diagnostics;
using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Databases.Postgres;
using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.Prompts;
using LangChain.Providers.Amazon.Bedrock.Predefined.Ai21Labs;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Providers.Amazon.Bedrock.Predefined.Anthropic;
using LangChain.Providers.Amazon.Bedrock.Predefined.Cohere;
using LangChain.Providers.Amazon.Bedrock.Predefined.Meta;
using LangChain.Providers.Amazon.Bedrock.Predefined.Stability;
using LangChain.Providers.HuggingFace;
using LangChain.Schema;
using LangChain.Sources;
using LangChain.Splitters.Text;
using Moq;
using Newtonsoft.Json;
using Npgsql;
using Resources;
using static LangChain.Chains.Chain;

namespace LangChain.Providers.Amazon.Bedrock.Tests;

[TestFixture, Explicit]
public class BedrockTests
{
    private Dictionary<string, float[]> EmbeddingsDict { get; } = new();
    private static string GenerateCollectionName() => "test-" + Guid.NewGuid().ToString("N");
    private string _connectionString;

    [Test]
    public void matrix_test()
    {
        int[,] matrix = new int[7, 15];
     Console.WriteLine(matrix.Length);
    }

    [Test]
    public async Task HF_Image_to_text()
    {
     const string ImageToTextModel = "Salesforce/blip-image-captioning-base";
     const string ImageFilePath = "test_image.jpg";

        using var client = new HttpClient();
        var provider = new HuggingFaceProvider(apiKey: string.Empty, client);
        var model = new HuggingFaceImageToTextModel(provider, ImageToTextModel);

      // ReadOnlyMemory<byte> imageData = await EmbeddedResource.ReadAllAsync(ImageFilePath);
        var path = Path.Combine(Path.GetTempPath(), "solar_system.png");
        var imageData = await File.ReadAllBytesAsync(path);
        var binaryData = new BinaryData(imageData, "image/png");

        var response = await model.GenerateTextFromImageAsync(new ImageToTextRequest
        {
            Image = binaryData
        });



        Console.WriteLine(response);
    }

    [Test]
    public async Task Chains()
    {
        var provider = new BedrockProvider();
        //var llm = new Jurassic2MidModel(provider);
        var llm = new ClaudeV21Model(provider);
        //var modelId = "amazon.titan-text-express-v1";
        // var modelId = "cohere.command-light-text-v14";

        var template = "What is a good name for a company that makes {product}?";
        var prompt = new PromptTemplate(new PromptTemplateInput(template, new List<string>(1) { "product" }));

        var chain = new LlmChain(new LlmChainInput(llm, prompt));

        var result = await chain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "fast cars" }
        }));

        Console.WriteLine("text: " + result.Value["text"]);
    }

    [Test]
    public async Task SequenceChainTests()
    {
        var provider = new BedrockProvider();
        var llm = new Jurassic2MidModel(provider);
        //var llm = new ClaudeV21Model(provider);
        //var llm = new Llama2With13BModel(provider);

        var firstTemplate = "What is a good name for a company that makes {product}?";
        var firstPrompt = new PromptTemplate(new PromptTemplateInput(firstTemplate, new List<string>(1) { "product" }));

        var chainOne = new LlmChain(new LlmChainInput(llm, firstPrompt)
        {
            Verbose = true,
            OutputKey = "company_name"
        });

        var secondTemplate = "Write a 20 words description for the following company:{company_name}";
        var secondPrompt =
            new PromptTemplate(new PromptTemplateInput(secondTemplate, new List<string>(1) { "company_name" }));

        var chainTwo = new LlmChain(new LlmChainInput(llm, secondPrompt));

        var overallChain = new SequentialChain(new SequentialChainInput(
            [
                chainOne,
                chainTwo
            ],
            ["product"],
            ["company_name", "text"]
        ));

        var result = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "colourful socks" }
        }));

        Console.WriteLine(result.Value["text"]);
    }

    [Test]
    public void LlmChainTest()
    {
        var provider = new BedrockProvider();
        var llm = new Jurassic2MidModel(provider);
        //var llm = new ClaudeV21Model(provider);
        //var llm = new Llama2With13BModel(provider);

        var promptText =
            @"You will be provided with information about pet. Your goal is to extract the pet name.

Information:
{information}

The pet name is 
";

        var chain =
            Set("My dog name is Bob", outputKey: "information")
            | Template(promptText, outputKey: "prompt")
            | LLM(llm, inputKey: "prompt", outputKey: "text");

        var res = chain.Run(resultKey: "text").Result;
        Console.WriteLine(res);
    }


    [Test]
    public void RetrievalChainTest()
    {
        var provider = new BedrockProvider();
        //var llm = new Jurassic2MidModel();
        //var llm = new ClaudeV21Model();
        var llm = new Llama2With13BModel(provider);
        var embeddings = new TitanEmbedTextV1Model(provider);
        var index = InMemoryVectorStore
            .CreateIndexFromDocuments(embeddings, new[]
            {
                "I spent entire day watching TV",
                "My dog's name is Bob",
                "This icecream is delicious",
                "It is cold in space"
            }.ToDocuments()).Result;

        string prompt1Text =
            @"Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.

{context}

Question: {question}
Helpful Answer:";

        var prompt2Text =
            @"Human will provide you with sentence about pet. You need to answer with pet name.

Human: My dog name is Jack
Answer: Jack
Human: I think the best name for a pet is ""Jerry""
Answer: Jerry
Human: {pet_sentence}
Answer: ";

        var chainQuestion =
            Set("What is the good name for a pet?", outputKey: "question")
            | RetrieveDocuments(index, inputKey: "question", outputKey: "documents")
            | StuffDocuments(inputKey: "documents", outputKey: "context")
            | Template(prompt1Text, outputKey: "prompt")
            | LLM(llm, inputKey: "prompt", outputKey: "pet_sentence");

        //  var chainQuestionRes = chainQuestion.Run(resultKey: "pet_sentence").Result;

        var chainFilter =
            // do not move the entire dictionary from the other chain
            chainQuestion.AsIsolated(outputKey: "pet_sentence")
            | Template(prompt2Text, outputKey: "prompt")
            | LLM(llm, inputKey: "prompt", outputKey: "text");

        var res = chainFilter.Run(resultKey: "text").Result;
        Console.WriteLine(res);
    }

    [Test]
    public async Task SimpleRag()
    {
        var provider = new BedrockProvider();
        //var llm = new Jurassic2MidModel();
        //var llm = new ClaudeV21Model();
        var llm = new TitanTextExpressV1Model(provider);
        var embeddings = new TitanEmbedTextV1Model(provider);

        PdfPigPdfSource pdfSource = new PdfPigPdfSource("x:\\Harry-Potter-Book-1.pdf");
        var documents = await pdfSource.LoadAsync();

        TextSplitter textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 200, chunkOverlap: 50);

        if (File.Exists("vectors.db"))
            File.Delete("vectors.db");

        if (!File.Exists("vectors.db"))
            await SQLiteVectorStore.CreateIndexFromDocuments(embeddings, documents, "vectors.db", "vectors", textSplitter: textSplitter);

        var vectorStore = new SQLiteVectorStore("vectors.db", "vectors", embeddings);
        var index = new VectorStoreIndexWrapper(vectorStore);

        string promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";


        var chain =
            Set("what color is the car?", outputKey: "question")                     // set the question
                                                                                     //Set("Hagrid was looking for the golden key.  Where was it?", outputKey: "question")                     // set the question
                                                                                     // Set("Who was on the Dursleys front step?", outputKey: "question")                     // set the question
                                                                                     // Set("Who was drinking a unicorn blood?", outputKey: "question")                     // set the question
            | RetrieveDocuments(index, inputKey: "question", outputKey: "documents", amount: 5) // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")                       // combine documents together and put them into context
            | Template(promptText)                                                              // replace context and question in the prompt with their values
            | LLM(llm);                                                                       // send the result to the language model

        var res = await chain.Run("text");
        Console.WriteLine(res);
    }

    [Test]
    public async Task CanGetImage()
    {
        var provider = new BedrockProvider();
        var model = new TitanImageGeneratorV1Model(provider);
        var response = await model.GenerateImageAsync("create a picture of the solar system");

        var path = Path.Combine(Path.GetTempPath(), "solar_system.png");
        Data image = response.Images[0];
        var images = response.Images.Select(x => x.ToByteArray()).ToList();

        await File.WriteAllBytesAsync(path, response.Images[0].ToByteArray());

        Process.Start(path);
    }

    [Test]
    public async Task CanGetImage2()
    {
        var provider = new BedrockProvider();
        var model = new StableDiffusionExtraLargeV1Model(provider);
        var response = await model.GenerateImageAsync(
            "i'm going to prepare a recipe.  show me an image of realistic food ingredients");

        var path = Path.Combine(Path.GetTempPath(), "food.png");

        await File.WriteAllBytesAsync(path, response.Images[0].ToByteArray());

        Process.Start(path);
    }

    [TestCase(true, false)]
    [TestCase(false, false)]
    [TestCase(true, true)]
    [TestCase(false, true)]
    public async Task SimpleTest(bool useStreaming, bool useChatSettings)
    {
        var provider = new BedrockProvider();
        var llm = new CommandLightTextV14Model(provider);

        llm.PromptSent += (_, prompt) => Console.WriteLine($"Prompt: {prompt}");
        llm.PartialResponseGenerated += (_, delta) => Console.Write(delta);
        llm.CompletedResponseGenerated += (_, prompt) => Console.WriteLine($"Completed response: {prompt}");

        var prompt = @"
you are a comic book writer.  you will be given a question and you will answer it. 
question: who are 10 of the most popular superheros and what are their powers?";

        if (useChatSettings)
        {
            var response = await llm.GenerateAsync(prompt, new BedrockChatSettings { UseStreaming = useStreaming });
            response.LastMessageContent.Should().NotBeNull();
        }
        else
        {
            var response = await llm.GenerateAsync(prompt);
            response.LastMessageContent.Should().NotBeNull();
        }
    }

    [Test]
    public async Task AttachToPostgres()
    {
        var provider = new BedrockProvider();
        var llm = new TitanTextExpressV1Model(provider);
        var embeddings = new TitanEmbedTextV1Model(provider);

        // PdfPigPdfSource pdfSource = new PdfPigPdfSource("x:\\Harry-Potter-Book-1.pdf");
        //var documents = await pdfSource.LoadAsync();

        TextSplitter textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 200, chunkOverlap: 50);

        const string host = "database-dev-1.cwrf01ytdgxr.us-east-1.rds.amazonaws.com";
        const int port = 5432;

        _connectionString = $"Host={host};Port={port};Database=test;User ID=postgres;Password=Kathmandu!123;";

        PopulateEmbedding();
        EnsureVectorExtensionAsync();

        using var httpClient = new HttpClient();
        var collectionName = GenerateCollectionName();
        // var embeddingsMock = CreateFakeEmbeddings();

        var db = new PostgresDbClient(_connectionString, "public", 1536);
        await db.CreateEmbeddingTableAsync(collectionName);
        var store = new PostgresVectorStore(_connectionString, 1526, embeddings, collectionName: collectionName);

        //var documents = new[]
        //{
        //    new Document("apple", new Dictionary<string, object>
        //    {
        //        ["color"] = "red"
        //    }),
        //    new Document("orange", new Dictionary<string, object>
        //    {
        //        ["color"] = "orange"
        //    })
        //};

        var texts = new[] { "hello world", "what's going on?" };
        var ids = await store.AddTextsAsync(texts);
    }

    private void EnsureVectorExtensionAsync()
    {
        var dataSource = new NpgsqlDataSourceBuilder(_connectionString).Build();
        var connection = dataSource.OpenConnection();
        using (connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "CREATE EXTENSION IF NOT EXISTS vector";

            command.ExecuteNonQuery();
        }
    }

    private void PopulateEmbedding()
    {
        foreach (var embeddingFile in Directory.EnumerateFiles("embeddings"))
        {
            var jsonRaw = File.ReadAllText(embeddingFile);
            var json =
                JsonConvert.DeserializeObject<Dictionary<string, float[]>>(jsonRaw) ??
                throw new InvalidOperationException("json is null");
            var kv = json.First();
            EmbeddingsDict.Add(kv.Key, kv.Value);
        }
    }

    private Mock<IEmbeddingModel> CreateFakeEmbeddings()
    {
        var mock = new Mock<IEmbeddingModel>();

        mock.Setup(x => x.CreateEmbeddingsAsync(
                It.IsAny<string>(),
                It.IsAny<EmbeddingSettings>(),
                It.IsAny<CancellationToken>()))
            .Returns<string, EmbeddingSettings, CancellationToken>(
                (query, _, _) =>
                {
                    var embedding = EmbeddingsDict.TryGetValue(query, out var value)
                        ? value
                        : throw new ArgumentException("not in dict");

                    return Task.FromResult(new EmbeddingResponse
                    {
                        Values = [embedding],
                        Usage = Usage.Empty,
                        UsedSettings = EmbeddingSettings.Default,
                    });
                });

        mock.Setup(x => x.CreateEmbeddingsAsync(
                It.IsAny<string[]>(),
                It.IsAny<EmbeddingSettings>(),
                It.IsAny<CancellationToken>()))
            .Returns<string[], EmbeddingSettings, CancellationToken>(
                (texts, _, _) =>
                {
                    var embeddings = new float[texts.Length][];

                    for (int index = 0; index < texts.Length; index++)
                    {
                        var text = texts[index];
                        embeddings[index] = EmbeddingsDict.TryGetValue(text, out var value)
                            ? value
                            : throw new ArgumentException("not in dict");
                    }

                    return Task.FromResult(new EmbeddingResponse
                    {
                        Values = embeddings,
                        Usage = Usage.Empty,
                        UsedSettings = EmbeddingSettings.Default,
                    });
                });

        return mock;
    }
}
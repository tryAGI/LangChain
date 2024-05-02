using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Providers.Amazon.Bedrock.Predefined.Anthropic;
using LangChain.DocumentLoaders;
using static LangChain.Chains.Chain;

namespace LangChain.Databases.IntegrationTests;

public partial class DatabaseTests
{
    #region Query Images

    private static async Task<DatabaseTestEnvironment> SetupImageTestsAsync()
    {
        var environment = await StartEnvironmentForAsync(SupportedDatabase.OpenSearch);
        environment.Dimensions = 1024;
        environment.EmbeddingModel = new TitanEmbedImageV1Model(new BedrockProvider())
        {
            Settings = new BedrockEmbeddingSettings
            {
                Dimensions = environment.Dimensions,
            }
        };

        return environment;
    }

    [Test]
    [Explicit]
    public async Task index_test_images()
    {
        await using var environment = await SetupImageTestsAsync();
        var vectorCollection = await environment.VectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, environment.Dimensions);

        string[] extensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tiff" };
        var files = Directory.EnumerateFiles(@"[images directory]", "*.*", SearchOption.AllDirectories)
            .Where(s => extensions.Any(ext => ext == Path.GetExtension(s)));

        var images = files.ToBinaryData();

        var documents = new List<Document>();

        foreach (BinaryData image in images)
        {
            var model = new Claude3HaikuModel(new BedrockProvider());
            var message = new Message(" \"what's this an image of and describe the details?\"", MessageRole.Human);

            var chatRequest = ChatRequest.ToChatRequest(message);
            chatRequest.Image = image;

            var response = await model.GenerateAsync(chatRequest);

            var document = new Document
            {
                PageContent = response,
                Metadata = new Dictionary<string, object>
                {
                    {response, image}
                }
            };

            documents.Add(document);
        }

        var pages = await vectorCollection.AddDocumentsAsync(environment.EmbeddingModel, documents);
    }

    [Test]
    [Explicit]
    public async Task can_query_image_against_images()
    {
        await using var environment = await SetupImageTestsAsync();
        var vectorCollection = await environment.VectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, environment.Dimensions);

        var path = Path.Combine(Path.GetTempPath(), "test_image.jpg");
        var imageData = await File.ReadAllBytesAsync(path);
        var binaryData = new BinaryData(imageData, "image/jpg");

        var embeddingRequest = new EmbeddingRequest
        {
            Strings = new List<string>(),
            Images = new List<Data> { Data.FromBytes(binaryData.ToArray()) }
        };
        var embedding = await environment.EmbeddingModel.CreateEmbeddingsAsync(embeddingRequest)
            .ConfigureAwait(false);

        var floats = embedding.ToSingleArray();
        var similaritySearchByVectorAsync = await vectorCollection.SearchAsync(floats).ConfigureAwait(false);

        Console.WriteLine("Count: " + similaritySearchByVectorAsync.Items.Count);
    }

    [Test]
    [Explicit]
    public async Task can_query_text_against_images()
    {
        await using var environment = await SetupImageTestsAsync();
        var vectorCollection = await environment.VectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, environment.Dimensions);

        var llm = new Claude3SonnetModel(new BedrockProvider());

        var promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";

        var chain =
            Set("tell me about the orange shirt", outputKey: "question")                     // set the question
            | RetrieveDocuments(vectorCollection, environment.EmbeddingModel, inputKey: "question", outputKey: "documents", amount: 10) // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")                       // combine documents together and put them into context
            | Template(promptText)                                                              // replace context and question in the prompt with their values
            | LLM(llm);                                                                       // send the result to the language model

        var res = await chain.RunAsync("text", CancellationToken.None);
        Console.WriteLine(res);
    }

    #endregion

    #region Query Simple Documents

    private static async Task<DatabaseTestEnvironment> SetupDocumentTestsAsync()
    {
        var environment = await StartEnvironmentForAsync(SupportedDatabase.OpenSearch);
        environment.Dimensions = 1536;
        environment.EmbeddingModel = new TitanEmbedTextV1Model(new BedrockProvider())
        {
            Settings = new BedrockEmbeddingSettings
            {
                Dimensions = environment.Dimensions,
            }
        };

        return environment;
    }

    [Test]
    [Explicit]
    public async Task index_test_documents()
    {
        await using var environment = await SetupDocumentTestsAsync();
        var vectorCollection = await environment.VectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, environment.Dimensions);

        var documents = new[]
        {
            "I spent entire day watching TV",
            "My dog's name is Bob",
            "The car is orange",
            "This icecream is delicious",
            "It is cold in space",
        }.ToDocuments();

        var pages = await vectorCollection.AddDocumentsAsync(environment.EmbeddingModel, documents);
        Console.WriteLine("pages: " + pages.Count);
    }

    [Test]
    [Explicit]
    public async Task can_query_test_documents()
    {
        await using var environment = await SetupDocumentTestsAsync();
        var vectorCollection = await environment.VectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, environment.Dimensions);

        var llm = new Claude3SonnetModel(new BedrockProvider());

        const string question = "what color is the car?";

        var promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";
        var chain =
            Set(question, outputKey: "question")
            | RetrieveDocuments(vectorCollection, environment.EmbeddingModel, inputKey: "question", outputKey: "documents", amount: 2)
            | StuffDocuments(inputKey: "documents", outputKey: "context")
            | Template(promptText)
            | LLM(llm);


        var res = await chain.RunAsync("text", CancellationToken.None);
        Console.WriteLine(res);
    }

    #endregion

    #region Query Pdf Book

    [Test]
    [Explicit]
    public async Task index_harry_potter_book()
    {
        await using var environment = await SetupDocumentTestsAsync();
        var vectorCollection = await environment.VectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, environment.Dimensions);

        var pdfSource = new PdfPigPdfLoader();
        var documents = await pdfSource.LoadAsync(DataSource.FromPath("x:\\Harry-Potter-Book-1.pdf"));

        var pages = await vectorCollection.AddDocumentsAsync(environment.EmbeddingModel, documents);
        Console.WriteLine("pages: " + pages.Count());
    }

    [Test]
    [Explicit]
    public async Task can_query_harry_potter_book()
    {
        await using var environment = await SetupDocumentTestsAsync();
        var vectorCollection = await environment.VectorDatabase.GetOrCreateCollectionAsync(VectorCollection.DefaultName, environment.Dimensions);

        var llm = new Claude3SonnetModel(new BedrockProvider());

        var promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";

        var chain =
            //Set("what color is the car?", outputKey: "question")                     // set the question
            //Set("Hagrid was looking for the golden key.  Where was it?", outputKey: "question")                     // set the question
            // Set("Who was on the Dursleys front step?", outputKey: "question")                     // set the question
            Set("Who was drinking a unicorn blood?", outputKey: "question")                     // set the question
            | RetrieveDocuments(vectorCollection, environment.EmbeddingModel, inputKey: "question", outputKey: "documents", amount: 10) // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")                       // combine documents together and put them into context
            | Template(promptText)                                                              // replace context and question in the prompt with their values
            | LLM(llm);                                                                       // send the result to the language model

        var res = await chain.RunAsync("text", CancellationToken.None);
        Console.WriteLine(res);
    }

    #endregion
}
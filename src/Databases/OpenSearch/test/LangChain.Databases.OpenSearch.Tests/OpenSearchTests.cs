using Amazon;
using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Sources;
using static LangChain.Chains.Chain;

namespace LangChain.Databases.OpenSearch.Tests;

//
// docker run -p 9200:9200 -p 9600:9600 -e "discovery.type=single-node" -e "plugins.security.disabled=true" -e "OPENSEARCH_INITIAL_ADMIN_PASSWORD=<custom-admin-password>" opensearchproject/opensearch:latest
//
public class OpenSearchTests
{
    private string? _indexName;
    private OpenSearchVectorStoreOptions? _options;
    private OpenSearchVectorStore? _vectorStore;
    private BedrockProvider? _provider;

    [SetUp]
    public void Setup()
    {
        _provider = new BedrockProvider();

        _indexName = "test-index";
        // var uri = new Uri("https://<your uri>.aos.us-east-1.on.aws");
        var uri = new Uri("http://localhost:9200");
        var password = Environment.GetEnvironmentVariable("OPENSEARCH_INITIAL_ADMIN_PASSWORD");
        _options = new OpenSearchVectorStoreOptions
        {
            IndexName = _indexName,
            ConnectionUri = uri,
            Username = "<your opensearch username>",
            Password = password
        };

        var embeddings = new TitanEmbedTextV1Model(_provider!);
        _vectorStore = new OpenSearchVectorStore(embeddings, _options);
    }

    #region Query Simple Documents

    [Test]
    public async Task index_test_documents()
    {
        var provider = new BedrockProvider(RegionEndpoint.USWest2);
        var embeddings = new TitanEmbedTextV1Model(provider);

        const string question = "what color is the car?";
        var embeddingsAsync = await embeddings.CreateEmbeddingsAsync(question);
        var vectors = embeddingsAsync.Values.Select(value => value[0]);

        var documents = new[]
        {
            "I spent entire day watching TV",
            "My dog's name is Bob",
            "The car is orange.",
            "This icecream is delicious",
            "It is cold in space",
        }.ToDocuments();

        var pages = await _vectorStore!.AddDocumentsAsync(documents);
        Console.WriteLine("pages: " + pages.Count());

        var similaritySearchByVectorAsync = _vectorStore?.SimilaritySearchByVectorAsync(vectors).ConfigureAwait(false);
        Console.WriteLine(similaritySearchByVectorAsync.Value);
    }

    [Test]
    public async Task can_query_test_documents()
    {
        var provider = new BedrockProvider(RegionEndpoint.USWest2);
        var llm = new TitanTextExpressV1Model(_provider!);
        var index = new VectorStoreIndexWrapper(_vectorStore!);

        const string question = "what color is the car?";

        var promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";
        var chain =
            Set(question, outputKey: "question")
            | RetrieveDocuments(index, inputKey: "question", outputKey: "documents", amount: 2)
            | StuffDocuments(inputKey: "documents", outputKey: "context")
            | Template(promptText)
            | LLM(llm);


        var res = await chain.Run("text");
        Console.WriteLine(res);
    }

    #endregion

    #region Query Pdf Book

    [Test]
    public async Task index_harry_potter_book()
    {
        var pdfSource = new PdfPigPdfSource("x:\\Harry-Potter-Book-1.pdf");
        var documents = await pdfSource.LoadAsync();

        var pages = await _vectorStore!.AddDocumentsAsync(documents);
        Console.WriteLine("pages: " + pages.Count());
    }

    [Test]
    public async Task can_query_harry_potter_book()
    {
        var llm = new TitanTextExpressV1Model(_provider!);
        var index = new VectorStoreIndexWrapper(_vectorStore!);

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
            | RetrieveDocuments(index, inputKey: "question", outputKey: "documents", amount: 10) // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")                       // combine documents together and put them into context
            | Template(promptText)                                                              // replace context and question in the prompt with their values
            | LLM(llm);                                                                       // send the result to the language model

        var res = await chain.Run("text");
        Console.WriteLine(res);
    }

    #endregion
}
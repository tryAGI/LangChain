using System.ClientModel;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.SqliteVec;
using OpenAI;

// Connect to Ollama via its OpenAI-compatible endpoint
var ollamaEndpoint = new Uri("http://localhost:11434/v1");
var ollamaOptions = new OpenAIClientOptions { Endpoint = ollamaEndpoint };
var ollamaApiKey = new ApiKeyCredential("ollama");

IChatClient llm = new OpenAIClient(ollamaApiKey, ollamaOptions)
    .GetChatClient("llama3").AsIChatClient();
IEmbeddingGenerator<string, Embedding<float>> embeddingModel = new OpenAIClient(ollamaApiKey, ollamaOptions)
    .GetEmbeddingClient("all-minilm").AsIEmbeddingGenerator();

var vectorStore = new SqliteVectorStore("Data Source=vectors.db");

var vectorCollection = await vectorStore.AddDocumentsFromAsync<PdfPigPdfLoader>(
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

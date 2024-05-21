using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Providers.Ollama;
using LangChain.Extensions;
using Ollama;

var provider = new OllamaProvider(options: new RequestOptions
{
    Stop = ["\n"],
    Temperature = 0.0f,
});
var embeddingModel = new OllamaEmbeddingModel(provider, id: "all-minilm");
var llm = new OllamaChatModel(provider, id: "llama3");

var vectorDatabase = new SqLiteVectorDatabase(dataSource: "vectors.db");

var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
    embeddingModel, // Used to convert text to embeddings
    dimensions: 384, // Should be 384 for all-minilm
    dataSource: DataSource.FromUrl("https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf"),
    collectionName: "harrypotter", // Can be omitted, use if you want to have multiple collections
    textSplitter: null,
    behavior: AddDocumentsToDatabaseBehavior.JustReturnCollectionIfCollectionIsAlreadyExists);


const string question = "What is Harry's Address?";
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
     """).ConfigureAwait(false);

Console.WriteLine($"LLM answer: {answer}");
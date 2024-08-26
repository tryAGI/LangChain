using LangChain.Databases.Sqlite;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.DocumentLoaders;
using LangChain.Providers.Ollama;
using LangChain.Providers.OpenAI;
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
        //// # Introduction
        //// This tutorial will help you setup a RAG (retrieval-augmented generation) application, where you can ask questions of a PDF document, such as the book [Harry Potter and the Philosopher's Stone](https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf).
        //// 
        //// # RAG
        //// The principle behind RAG is as follows.
        //// 
        //// First, you index the documents of interest.
        //// - Split your document into smaller text snippets or chunks
        //// - Generate an embedding for each chunk.  An embedding is a vector (array of numbers) that encodes the semantic meaning of the text
        //// - Save these embeddings into a special-purpose database, which enables similarity search
        //// 
        //// Second, you ask your question.
        //// - Generate an embedding for the question text
        //// - Perform a similarity search in your database using that embedding, which will return chunks that are semantically related to the question
        //// - Pass the chunks and the question to the LLM, along with other prompt text for guidance
        //// - Receive a nicely-worded response from the LLM that hopefully answers the question
        //// - PROFIT!
        //// 
        //// # Project
        //// To get started, create a new console application and add the following nuget packages.  (Use the pre-release checkbox.)
        //// ```
        //// LangChain
        //// LangChain.Providers.Ollama
        //// LangChain.Databases.Sqlite
        //// LangChain.Sources.Pdf
        //// ```
        //// The code here assumes you are using .NET 8.
        //// 
        //// # Models
        //// 
        //// Lets configure the models.
        //// 
        //// ## Completion and Embeddings
        //// You can use either OpenAI (online) or Ollama (offline):
        //// 
        //// ### OpenAI
        //// To use this chat and embedding model, you will need an API key from OpenAI.  This has non-zero cost.

        // prepare OpenAI embedding model
        var provider = new OpenAiProvider(apiKey:
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InvalidOperationException("OPENAI_API_KEY key is not set"));
        var embeddingModel = new TextEmbeddingV3SmallModel(provider);
        var llm = new OpenAiLatestFastChatModel(provider);

        //// ### Ollama
        //// To use this chat and embedding model, you will need an Ollama instance running.
        //// This is free, assuming it is running locally--this code assumes it is available at https://localhost:11434.

        // prepare Ollama with mistral model
        var providerOllama = new OllamaProvider(
            options: new RequestOptions
            {
                Stop = ["\n"],
                Temperature = 0.0f,
            });
        var embeddingModelOllama = new OllamaEmbeddingModel(providerOllama, id: "nomic-embed-text");
        var llmOllama = new OllamaChatModel(providerOllama, id: "llama3.1").UseConsoleForDebug();

        //// Configure the vector database.

        using var vectorDatabase = new SqLiteVectorDatabase("vectors.db");
        var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
            embeddingModel,
            dimensions: 1536, // Should be 1536 for TextEmbeddingV3SmallModel
                              // First, specify the source to index.
            dataSource: DataSource.FromPath("E:\\AI\\Datasets\\Books\\Harry-Potter-Book-1.pdf"),
            collectionName: "harrypotter",
            // Second, configure how to extract chunks from the bigger document.
            textSplitter: new RecursiveCharacterTextSplitter(
                chunkSize: 500, // To pick the chunk size, estimate how much information would be required to capture most passages you'd like to ask questions about.  Too many characters makes it difficult to capture semantic meaning, and too few characters means you are more likely to split up important points that are related. In general, 200-500 characters is good for stories without complex sequences of actions.
                chunkOverlap: 200)); // To pick the chunk overlap you need to estimate the size of the smallest piece of information. It may happen that one chunk ends with `Ron's hair` and the other one starts with `is red`.In this case, an embedding would miss important context, and not be generated propperly. With overlap the end of the first chunk will appear in the begining of the other, eliminating the problem.

        //// # Execution
        //// 
        //// Now we will put together the chain of actions that will run whenever we have questions about the document.
        //// 
        //// ## Prompt
        //// 
        //// Chains require a specially-crafted prompt. For our task we will use the one below. Feel free to tweak this for your purposes.

        string promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";

        //// Inside the prompt we have 2 placeholders: `context` and `question`, which will be replaced during execution.
        //// 
        //// - question: The question asked by the user
        //// - context: The document chunks that were found to be semantically related to the question being asked
        //// 
        //// ## Chain
        //// 
        //// Now we can configure the chain of actions that should occur during execution.

        var chain =
            Set("Who was drinking a unicorn blood?", outputKey: "question")     // set the question
            | RetrieveDocuments(
                vectorCollection,
                embeddingModel,
                inputKey: "question",
                outputKey: "documents",
                amount: 5)                                                      // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")       // combine documents together and put them into context
            | Template(promptText)                                              // replace context and question in the prompt with their values
            | LLM(llm);                                                         // send the result to the language model

        // get chain result
        var result = await chain.RunAsync("text", CancellationToken.None);

        Console.WriteLine(result);

        //// We are done! Since we previously registered for events on the completion model, the output will be printed automatically.
        //// 
        //// # Example
        //// 
        //// Here is an example execution output.
        //// 
        //// - The first paragraph is part of our prompt template.
        //// - The next 5 paragraphs are semantically-related context.
        //// - The "Question:" line shows the question we asked.
        //// - The "Helpful Answer:" line shows how the completion model answered the question we asked.
        //// 
        //// ```
        //// Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.
        //// 
        //// and began to drink its blood. 'AAAAAAAAAAARGH!' Malfoy let out a terrible scream and bolted - so did Fang. The hooded figure raised its head and looked right at Harry - unicorn blood was dribbling
        //// 
        //// close by. There were still spots of unicorn blood here and there along the winding path.
        //// 
        //// is because it is a monstrous thing, to slay a unicorn,' said Firenze. 'Only one who has nothing to lose, and everything to gain, would commit such a crime. The blood of a unicorn will keep you alive,
        //// 
        //// beast. Harry, Malfoy and Fang stood transfixed. The cloaked figure reached the uni-corn, it lowered its head over the wound in the animal's side, and began to drink its blood. 'AAAAAAAAAAARGH!'
        //// 
        //// light breeze lifted their hair as they looked into the Forest. 'Look there,' said Hagrid, 'see that stuff shinin' on the ground? Silvery stuff? That's unicorn blood. There's a unicorn in there bin
        //// 
        //// Question: Who was drinking a unicorn blood?
        //// Helpful Answer: The hooded figure was drinking a unicorn's blood.
        //// ```
        //// 
        //// As you can see, every piece of context is mentioning unicorn blood. In the first piece the hooded figure is mentioned. So the model takes this as an answer to our question.
        //// 
        //// Now go and try to ask your own questions!
    }
}
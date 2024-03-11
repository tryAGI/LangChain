using LangChain.Indexes;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Sources;
using static LangChain.Chains.Chain;

namespace LangChain.Databases.OpenSearch.Tests;

public class RagOpenSearchTests
{
    [Test]
    public async Task can_create_rag()
    {
        var provider = new BedrockProvider();
        var llm = new TitanTextExpressV1Model(provider);
        var embeddings = new TitanEmbedTextV1Model(provider);

        var pdfSource = new PdfPigPdfSource("x:\\Harry-Potter-Book-1.pdf");
        var documents = await pdfSource.LoadAsync();

        var password = Environment.GetEnvironmentVariable("OPENSEARCH_INITIAL_ADMIN_PASSWORD");
        var options = new OpenSearchVectorStoreOptions
        {
            IndexName = "test-index",
            ConnectionUri = new Uri("https://search-myopensearch-dvy73fcpabi2dch3xtxvxxooau.aos.us-east-1.on.aws"),
            Username = "taugustj",
            Password = password
        };

        var vectorStore = new OpenSearchVectorStore(embeddings, options);
        await vectorStore.AddDocumentsAsync(documents);
        var index = new VectorStoreIndexWrapper(vectorStore);

        var promptText =
            @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible.

{context}

Question: {question}
Helpful Answer:";

        var chain =
             // Set("what color is the car?", outputKey: "question")                     // set the question
             //Set("Hagrid was looking for the golden key.  Where was it?", outputKey: "question")                     // set the question
             // Set("Who was on the Dursleys front step?", outputKey: "question")                     // set the question
             Set("Who was drinking a unicorn blood?", outputKey: "question")                     // set the question
            | RetrieveDocuments(index, inputKey: "question", outputKey: "documents", amount: 5) // take 5 most similar documents
            | StuffDocuments(inputKey: "documents", outputKey: "context")                       // combine documents together and put them into context
            | Template(promptText)                                                              // replace context and question in the prompt with their values
            | LLM(llm);                                                                       // send the result to the language model

        var res = await chain.Run("text");
        Console.WriteLine(res);

        return;
    }
}
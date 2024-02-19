using LangChain.Base;
using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.Prompts;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Providers.Amazon.Bedrock.Predefined.Meta;
using LangChain.Providers.Amazon.Bedrock.Predefined.Cohere;
using LangChain.Providers.Amazon.SageMaker;
using LangChain.Schema;
using LangChain.Sources;
using LangChain.TextSplitters;
using static LangChain.Chains.Chain;

namespace LangChain.Providers.Amazon.IntegrationTests;

[TestFixture, Explicit]
public class SageMakerTests
{
    [Test]
    public async Task SimpleChain()
    {
        var provider = new SageMakerProvider(apiGatewayEndpoint: "https://your-url.execute-api.us-east-1.amazonaws.com/model");
        var model = new SageMakerChatModel(provider, sageMakerEndpointName: "openchat");

        var prompt = @"what's supermans parents names?";

        var chain =
            Set(prompt)
            | LLM(model);

        Console.WriteLine(await chain.Run("text"));
    }

    [Test]
    public async Task Chains()
    {
        var provider = new SageMakerProvider(apiGatewayEndpoint: "https://your-url.execute-api.us-east-1.amazonaws.com/model");
        var model = new SageMakerChatModel(provider, sageMakerEndpointName: "openchat");

        var template = "What is a good name for a company that makes {product}?";
        var prompt = new PromptTemplate(new PromptTemplateInput(template, new List<string>(1) { "product" }));

        var chain = new LlmChain(new LlmChainInput(model, prompt));

        var result = await chain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "fast cars" }
        }));

        Console.WriteLine("text: " + result.Value["text"]);
    }

    [Test]
    public async Task SequenceChainTests()
    {
        var provider = new SageMakerProvider(apiGatewayEndpoint: "https://your-url.execute-api.us-east-1.amazonaws.com/model");
        var model = new SageMakerChatModel(provider, sageMakerEndpointName: "openchat");

        var firstTemplate = "What is a good name for a company that makes {product}?";
        var firstPrompt = new PromptTemplate(new PromptTemplateInput(firstTemplate, new List<string>(1) { "product" }));

        var chainOne = new LlmChain(new LlmChainInput(model, firstPrompt)
        {
            Verbose = true,
            OutputKey = "company_name"
        });

        var secondTemplate = "Write a 20 words description for the following company:{company_name}";
        var secondPrompt =
            new PromptTemplate(new PromptTemplateInput(secondTemplate, new List<string>(1) { "company_name" }));

        var chainTwo = new LlmChain(new LlmChainInput(model, secondPrompt));

        var overallChain = new SequentialChain(new SequentialChainInput(
            new[]
            {
                chainOne,
                chainTwo
            },
            new[] { "product" },
            new[] { "company_name", "text" }
        ));

        var result = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "colourful socks" }
        }));

        Console.WriteLine(result.Value["text"]);
    }

    [Test]
    public void LLMChainTest()
    {
        var provider = new SageMakerProvider(apiGatewayEndpoint: "https://your-url.execute-api.us-east-1.amazonaws.com/model");
        var model = new SageMakerChatModel(provider, sageMakerEndpointName: "openchat");

        var promptText =
            @"You will be provided with information about pet. Your goal is to extract the pet name.

Information:
{information}

The pet name is 
";

        var chain =
            Set("My dog name is Bob", outputKey: "information")
            | Template(promptText, outputKey: "prompt")
            | LLM(model, inputKey: "prompt", outputKey: "text");

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
        var embeddings = new EmbedEnglishV3Model(provider);

        var documents = new string[]
        {
            "I spent entire day watching TV",
            "My dog's name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        }.ToDocuments();
        var index = InMemoryVectorStore
            .CreateIndexFromDocuments(embeddings, documents).Result;

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
}
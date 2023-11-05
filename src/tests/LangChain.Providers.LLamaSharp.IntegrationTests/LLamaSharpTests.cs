using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Providers.Downloader;
using LangChain.Providers.LLamaSharp;
using LangChain.TextSplitters;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestClass]
public class LLamaSharpTests
{
    string ModelPath=>HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf","main").Result;
    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void PrepromptTest()
    {
        var model = new LLamaSharpModelChat(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
        });

        var response=model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are simple assistant. If human say 'Bob' then you will respond with 'Jack'.".AsSystemMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
        })).Result;

        Assert.AreEqual("Jack",response.Messages.Last().Content );

    }

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void InstructionTest()
    {
        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature=0
        });

        var response = model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are a calculator. Print the result of this expression: 2 + 2.".AsSystemMessage(),
            "Result:".AsSystemMessage(),
        })).Result;

        Assert.AreEqual("4",response.Messages.Last().Content.Trim());

    }

    float VectorDistance(float[] a, float[] b)
    {
        float result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result += (a[i] - b[i]) * (a[i] - b[i]);
        }

        return result;

    }
    
    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void EmbeddingsTestWithInMemory()
    {
        var embeddings = new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });

        string[] texts = new string[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        };

        InMemoryVectorStore vectorStore = new InMemoryVectorStore(embeddings);
        vectorStore.AddTextsAsync(texts).Wait();

        var query = "How do you call your pet?";
        var closest = vectorStore.SimilaritySearchAsync(query,k:1).Result.First();

        Assert.AreEqual("My dog name is Bob", closest.PageContent);
    }

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void DocumentsQuestionAnsweringTest()
    {
        // setup
        var embeddings = new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });

        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });

        InMemoryVectorStore vectorStore = new InMemoryVectorStore(embeddings);
        string[] texts = new string[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        };
        var textSplitter = new CharacterTextSplitter();
        VectorStoreIndexCreator indexCreator = new VectorStoreIndexCreator(vectorStore, textSplitter); 
        var index=indexCreator.FromDocumentsAsync(texts.Select(x=>new Document(x)).ToList()).Result;

        string prompt = "Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.\n\n{context}\n\nQuestion: {question}\nHelpful Answer:";
        var template = new PromptTemplate(new PromptTemplateInput(prompt, new List<string>() { "context", "question" }));
        var chain = new LlmChain(new LlmChainInput(model, template)
        {
            OutputKey = "text"
        });
        var stuffDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(chain)
        {
            InputKey = "context",
            DocumentVariableName = "context",
            OutputKey = "text",
        });


        // test
        var query = "What is the good name for a pet? Tell me only the name, no explanations.";
        var answer=index.QueryAsync(query, stuffDocumentsChain,
            documentsKey: "context",
            questionKey: "question",
            outputKey: "text").Result;
            



        Assert.IsTrue(answer.Contains("Bob"));
    }


}
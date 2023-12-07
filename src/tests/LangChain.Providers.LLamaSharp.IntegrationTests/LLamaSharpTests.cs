using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Embeddings.Base;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Chains.RetrievalQA;
using LangChain.Chains.Sequentials;
using LangChain.Databases.InMemory;
using LangChain.Prompts;
using LangChain.Providers.Downloader;
using LangChain.VectorStores;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public partial class LLamaSharpTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;
    
    [Test]
    [Explicit]
    public void PrepromptTest()
    {
        var model = new LLamaSharpModelChat(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
        });

        var response = model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are simple assistant. If human say 'Bob' then you will respond with 'Jack'.".AsSystemMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
        })).Result;

        Assert.AreEqual("Jack", response.Messages.Last().Content);

    }

    [Test]
    [Explicit]
    public void InstructionTest()
    {
        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });

        var response = model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are a calculator. Print the result of this expression: 2 + 2.".AsSystemMessage(),
            "Result:".AsSystemMessage(),
        })).Result;

        Assert.AreEqual("4", response.Messages.Last().Content.Trim());

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

    [Test]
    [Explicit]
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
        var closest = vectorStore.SimilaritySearchAsync(query, k: 1).Result.First();

        Assert.AreEqual("My dog name is Bob", closest.PageContent);
    }

    [Test]
    [Explicit]
    public void DocumentsQuestionAnsweringTest()
    {
        // setup
        var embeddings = CreateEmbeddings();
        var model = CreateInstructionModel();

        string[] texts = new string[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        };

        var index = CreateVectorStoreIndex(embeddings, texts);
        var template = CreatePromptTemplate();

        var chain = new LlmChain(new LlmChainInput(model, template));

        var stuffDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(chain)
        {
            DocumentVariableName = "context", // variable name in prompt template
                                              // for the documents
        });


        // test
        var question = "What is the good name for a pet?";
        var answer = index.QueryAsync(question, stuffDocumentsChain,
            inputKey: "question" // variable name in prompt template for the question
                                 // it would be passed by to stuffDocumentsChain
            ).Result ?? string.Empty;


        Assert.IsTrue(answer.Contains("Bob"));
    }

    private IChain CreateChain1(IChatModel model, IEmbeddings embeddings)
    {
        string[] texts = {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        };

        var index = CreateVectorStoreIndex(embeddings, texts);
        var template = CreatePromptTemplate();

        var llmchain = new LlmChain(new LlmChainInput(model, template)
        {
            OutputKey = "pet_sentence",
        });

        var stuffDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(llmchain)
        {
            DocumentVariableName = "context",

        });

        var chain = new RetrievalQaChain(
            new RetrievalQaChainInput(
                stuffDocumentsChain,
                index.Store.AsRetriever())
            {
                InputKey = "question",
                OutputKey = "pet_sentence",
            }
        );

        return chain;
    }

    [Test]
    [Explicit]
    public void SequentialChainTest()
    {
        // setup
        var embeddings = CreateEmbeddings();
        var model = CreateInstructionModel();

        var chain1 = CreateChain1(model, embeddings);

        var prompt =
            @"Human will provide you with sentence about pet. You need to answer with pet name.
Human: My dog name is Jack
Answer: Jack
Human: I think the best name for a pet is ""Jerry""
Answer: Jerry
Human: {pet_sentence}
Answer:";
        var template = new PromptTemplate(new PromptTemplateInput(prompt, new List<string>() { "pet_sentence" }));
        var chain2 = new LlmChain(new LlmChainInput(model, template));

        var sequence = new SequentialChain(
            new SequentialChainInput(
                new[] { chain1, chain2 },
                inputVariables: new[] { "question" }));

        var answer = sequence.Run("What is the good name for a pet?").Result;

        Assert.AreEqual("Bob", answer);
    }
}
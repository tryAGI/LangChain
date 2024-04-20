using LangChain.Abstractions.Chains.Base;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Chains.RetrievalQA;
using LangChain.Chains.Sequentials;
using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Indexes;
using LangChain.Prompts;
using LangChain.Providers.HuggingFace.Downloader;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public partial class LLamaSharpTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;
    
    [Test]
    [Explicit]
    public async Task PrepromptTest()
    {
        var model = new LLamaSharpModelChat(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
        });

        var response = await model.GenerateAsync(new[] {
            "You are simple assistant. If human say 'Bob' then you will respond with 'Jack'.".AsSystemMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
        });

        response.Messages.Last().Content.Should().Be("Jack");
    }

    [Test]
    [Explicit]
    public async Task InstructionTest()
    {
        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0
        });

        var response = await model.GenerateAsync(new[]
        {
            "You are a calculator. Print the result of this expression: 2 + 2.".AsSystemMessage(),
            "Result:".AsSystemMessage(),
        });

        response.Messages.Last().Content.Trim().Should().Be("4");
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
    public async Task EmbeddingsTestWithInMemory()
    {
        var embeddings = new LLamaSharpEmbeddings(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature = 0,
        });

        var vectorStore = new InMemoryVectorStore();
        await vectorStore.AddTextsAsync(embeddings, [
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        ]);

        var closest = (await vectorStore.SearchAsync(
            embeddings,
            "How do you call your pet?",
            searchSettings: new VectorSearchSettings
            {
                NumberOfResults = 1,
            })).Items[0];

        closest.Text.Should().Be("My dog name is Bob");
    }

    [Test]
    [Explicit]
    public async Task DocumentsQuestionAnsweringTest()
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

        var vectorDatabase = await CreateVectorStoreIndex(embeddings, texts);
        var template = CreatePromptTemplate();

        var chain = new LlmChain(new LlmChainInput(model, template));

        var stuffDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(chain)
        {
            DocumentVariableName = "context", // variable name in prompt template
                                              // for the documents
        });


        // test
        var question = "What is the good name for a pet?";
        var answer = vectorDatabase.QueryAsync(embeddings, question, stuffDocumentsChain,
            inputKey: "question" // variable name in prompt template for the question
                                 // it would be passed by to stuffDocumentsChain
            ).Result ?? string.Empty;

        answer.Contains("Bob").Should().BeTrue();
    }

    private async Task<IChain> CreateChain1(IChatModel model, IEmbeddingModel embeddings)
    {
        string[] texts = {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This icecream is delicious",
            "It is cold in space"
        };

        var index = await CreateVectorStoreIndex(embeddings, texts);
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
                index.AsRetriever(embeddings))
            {
                InputKey = "question",
                OutputKey = "pet_sentence",
            }
        );

        return chain;
    }

    [Test]
    [Explicit]
    public async Task SequentialChainTest()
    {
        // setup
        var embeddings = CreateEmbeddings();
        var model = CreateInstructionModel();

        var chain1 = await CreateChain1(model, embeddings);

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
                [chain1, chain2],
                inputVariables: ["question"]));

        var answer = sequence.Run("What is the good name for a pet?").Result;

        answer.Should().Be("Bob");
    }
}
using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.Sources;
using LangChain.Providers.HuggingFace.Downloader;
using static LangChain.Chains.Chain;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public class ChainTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [Test]
    public async Task PromptTest()
    {
        var chain =
            Set("World", outputKey: "var2")
            | Set("Hello", outputKey: "var1")
            | Template("{var1}, {var2}", outputKey: "prompt");

        var res = await chain.RunAsync(resultKey: "prompt", CancellationToken.None);

        res.Should().Be("Hello, World");
    }

    [Test]
    [Explicit]
    public async Task LlmChainTest()
    {
        var llm = LLamaSharpModelInstruction.FromPath(ModelPath);
        var promptText =
            @"You will be provided with information about pet. Your goal is to extract the pet name.

Information:
{information}

The pet name is 
";

        var chain =
            Set("My dog name is Bob", outputKey: "information")
            | Template(promptText, outputKey: "prompt")
            | LLM(llm, inputKey: "prompt", outputKey: "text");

        var res = await chain.RunAsync(resultKey: "text", CancellationToken.None);

        res.Should().Be("Bob");
    }

    [Test]
    [Explicit]
    public async Task RetrievalChainTest()
    {
        var llm = LLamaSharpModelInstruction.FromPath(ModelPath);
        var embeddings = LLamaSharpEmbeddings.FromPath(ModelPath);
        var vectorStore = new InMemoryVectorCollection();
        await vectorStore
            .AddDocumentsAsync(embeddings, new[]
            {
                "I spent entire day watching TV",
                "My dog name is Bob",
                "This icecream is delicious",
                "It is cold in space"
            }.ToDocuments());

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
            | RetrieveDocuments(vectorStore, embeddings, inputKey: "question", outputKey: "documents")
            | StuffDocuments(inputKey: "documents", outputKey: "context")
            | Template(prompt1Text, outputKey: "prompt")
            | LLM(llm, inputKey: "prompt", outputKey: "pet_sentence");

        var chainFilter =
            // do not move the entire dictionary from the other chain
            chainQuestion.AsIsolated(outputKey: "pet_sentence")
            | Template(prompt2Text, outputKey: "prompt")
            | LLM(llm, inputKey: "prompt", outputKey: "text");


        var res = await chainFilter.RunAsync(resultKey: "text", CancellationToken.None);
        res.Should().Be("Bob");
    }
}
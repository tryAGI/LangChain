using System.Diagnostics;
using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Databases.InMemory;
using LangChain.Docstore;
using LangChain.Prompts;
using LangChain.Providers.Amazon.Bedrock.Predefined.Ai21Labs;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Providers.Amazon.Bedrock.Predefined.Meta;
using LangChain.Providers.Amazon.Bedrock.Predefined.Stability;
using LangChain.Schema;
using static LangChain.Chains.Chain;

namespace LangChain.Providers.Amazon.Bedrock.IntegrationTests;

[TestFixture, Explicit]
public class BedrockTests
{
    [Test]
    public async Task Chains()
    {
        //var llm = new Jurassic2MidModel();
        //var llm = new ClaudeV21Model();
        var llm = new Llama2With13BModel();

        var template = "What is a good name for a company that makes {product}?";
        var prompt = new PromptTemplate(new PromptTemplateInput(template, new List<string>(1) { "product" }));

        var chain = new LlmChain(new LlmChainInput(llm, prompt));

        var result = await chain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "fast cars" }
        }));

        Console.WriteLine("text: " + result.Value["text"]);
    }

    [Test]
    public async Task SequenceChainTests()
    {
        var llm = new Jurassic2MidModel();
        //var llm = new ClaudeV21Model();
        //var llm = new Llama2With13BModel();

        var firstTemplate = "What is a good name for a company that makes {product}?";
        var firstPrompt = new PromptTemplate(new PromptTemplateInput(firstTemplate, new List<string>(1) { "product" }));

        var chainOne = new LlmChain(new LlmChainInput(llm, firstPrompt)
        {
            Verbose = true,
            OutputKey = "company_name"
        });

        var secondTemplate = "Write a 20 words description for the following company:{company_name}";
        var secondPrompt =
            new PromptTemplate(new PromptTemplateInput(secondTemplate, new List<string>(1) { "company_name" }));

        var chainTwo = new LlmChain(new LlmChainInput(llm, secondPrompt));

        var overallChain = new SequentialChain(new SequentialChainInput(
            [
                chainOne,
                chainTwo
            ],
            ["product"],
            ["company_name", "text"]
        ));

        var result = await overallChain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "colourful socks" }
        }));

        Console.WriteLine(result.Value["text"]);
    }

    [Test]
    public void LlmChainTest()
    {
        var llm = new Jurassic2MidModel();
        //var llm = new ClaudeV21Model();
        //var llm = new Llama2With13BModel();

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

        var res = chain.Run(resultKey: "text").Result;
        Console.WriteLine(res);
    }


    [Test]
    public void RetrievalChainTest()
    {
        //var llm = new Jurassic2MidModel();
        //var llm = new ClaudeV21Model();
        var llm = new Llama2With13BModel();
        var embeddings = new TitanEmbedTextV1Model();
        var index = InMemoryVectorStore
            .CreateIndexFromDocuments(embeddings, new[]
            {
                "I spent entire day watching TV",
                "My dog's name is Bob",
                "This icecream is delicious",
                "It is cold in space"
            }.ToDocuments()).Result;

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
    public async Task CanGetImage()
    {
        var model = new StableDiffusionExtraLargeV0Model();
        var response = await model.GenerateImageAsync(
            "create a picture of the solar system");

        var path = Path.Combine(Path.GetTempPath(), "solar_system.png");
        
        await File.WriteAllBytesAsync(path, response.Bytes);
        
        Process.Start(path);
    }
}
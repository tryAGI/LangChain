using LangChain.Chains.LLM;
using LangChain.Chains.Sequentials;
using LangChain.Databases.InMemory;
using LangChain.Docstore;
using LangChain.Prompts;
using LangChain.Providers.Bedrock.Embeddings;
using LangChain.Providers.Bedrock.Models;
using LangChain.Schema;
using static LangChain.Chains.Chain;

namespace LangChain.Providers.Bedrock.IntegrationTests;

[TestFixture, Explicit]
public class BedrockTests
{
    [Test]
    public async Task Chains()
    {
        //var modelId = "ai21.j2-mid-v1";
        //  var modelId = "anthropic.claude-v2:1";
        var modelId = "meta.llama2-13b-chat-v1";

        var model = new BedrockModel(modelId);

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
        var modelId = "ai21.j2-mid-v1";
        //  var modelId = "anthropic.claude-v2:1";
        // var modelId = "meta.llama2-13b-chat-v1";
        var model = new BedrockModel(modelId);

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
        var modelId = "ai21.j2-mid-v1";
        // var modelId = "anthropic.claude-v2:1";
        //var modelId = "meta.llama2-13b-chat-v1";
        var llm = new BedrockModel(modelId);

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
    public void RetreivalChainTest()
    {
        //var modelId = "ai21.j2-mid-v1";
        // var modelId = "anthropic.claude-v2:1";
        var modelId = "meta.llama2-13b-chat-v1";
        var llm = new BedrockModel(modelId);
        var embeddings = new BedrockEmbeddings("amazon.titan-embed-text-v1");

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
    public async Task CanGetImage()
    {
        var model = new BedrockModel("stability.stable-diffusion-xl-v0");

        var prompt = "create a picture of the solar system";

        var response = model.GenerateAsync(new ChatRequest(new[] { prompt.AsHumanMessage() })).Result;

        Console.WriteLine(response);
    }
}
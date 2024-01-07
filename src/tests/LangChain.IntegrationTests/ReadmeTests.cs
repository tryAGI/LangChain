using LangChain.Databases.InMemory;
using LangChain.Docstore;
using LangChain.Providers.OpenAI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public class ReadmeTests
{
    [Explicit]
    [Test]
    public async Task Readme()
    {
        var gpt4 = new Gpt4Model(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));
        var index = await InMemoryVectorStore.CreateIndexFromDocuments(gpt4, new[]
        {
            "I spent entire day watching TV",
            "My dog name is Bob",
            "This ice cream is delicious",
            "It is cold in space"
        }.ToDocuments());

        var chain = (
            Set("What is the good name for a pet?", outputKey: "question") |
            RetrieveDocuments(index, inputKey: "question", outputKey: "documents") |
            StuffDocuments(inputKey: "documents", outputKey: "context") |
            Template("""
                Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.

                {context}

                Question: {question}
                Helpful Answer:
                """, outputKey: "prompt") |
            LLM(gpt4, inputKey: "prompt", outputKey: "pet_sentence")) >>
            Template("""
                Human will provide you with sentence about pet. You need to answer with pet name.

                Human: My dog name is Jack
                Answer: Jack
                Human: I think the best name for a pet is "Jerry"
                Answer: Jerry
                Human: {pet_sentence}
                Answer:
                """, outputKey: "prompt") |
            LLM(gpt4, inputKey: "prompt", outputKey: "text");

        var result = await chain.Run(resultKey: "text");
        result.Should().Be("Bob");
    }
}
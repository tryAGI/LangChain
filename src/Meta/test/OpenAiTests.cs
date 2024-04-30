using LangChain.Chains.LLM;
using LangChain.Prompts;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.Schema;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class OpenAiTests
{
    [Test]
    public async Task TestOpenAi_WithValidInput_ShouldReturnResponse()
    {
        var model = new Gpt35TurboModel(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));

        string result = await model.GenerateAsync("What is a good name for a company that sells colourful socks?");

        result.Should().NotBeEmpty();

        Console.WriteLine(result);
    }

    [Test]
    public async Task TestOpenAi_WithChain_ShouldReturnResponse()
    {
        var llm = new Gpt35TurboModel(
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY is not set"));

        var template = "What is a good name for a company that makes {product}?";
        var prompt = new PromptTemplate(new PromptTemplateInput(template, new List<string>(1) { "product" }));

        var chain = new LlmChain(new LlmChainInput(llm, prompt));

        var result = await chain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "colourful socks" }
        }));

        // The result is an object with a `text` property.
        result.Value["text"].ToString().Should().NotBeEmpty();

        Console.WriteLine(result.Value["text"]);
    }
}
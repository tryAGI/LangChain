/*
using LangChain.Chains.LLM;
using LangChain.LLMS.AzureOpenAi;
using LangChain.Prompts;
using LangChain.Schema;

 Temporarily disabled Azure tests as there is no API key.
namespace LangChain.IntegrationTests
{
    public class AzureOpenAiTests
    {
        [Fact]
        public async Task TestOpenAi_WithValidInput_ShouldReturnResponse()
        {
            var model = new AzureOpenAi();

            var result = await model.Call("What is a good name for a company that sells colourful socks?");

            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task TestOpenAi_WithChain_ShouldReturnResponse()
        {
            var llm = new AzureOpenAi();

            var template = "What is a good name for a company that makes {product}?";
            var prompt = new PromptTemplate(new PromptTemplateInput(template, new List<string>(1) { "product" }));

            var chain = new LlmChain(new LlmChainInput(llm, prompt));

            var result = await chain.Call(new ChainValues(new Dictionary<string, object>(1)
        {
            { "product", "colourful socks" }
        }));

            // The result is an object with a `text` property.
            (result.Value["text"] as string).Should().NotBeEmpty();
        }
    }
}
*/
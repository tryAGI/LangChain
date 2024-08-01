using LangChain.Providers.OpenAI.Predefined;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task HowToUseOpenAiProvider()
    {
        var model = new Gpt35TurboModel("your_openAI_key");
        var chain =
            Set("Hello!", outputKey: "request")          // set context variable `request` to "Hello"
            | LLM(model, inputKey: "request", outputKey: "text"); // get text from context variable `request`, pass it to the model and put result into `text`

        var result = await chain.RunAsync("text", CancellationToken.None);  // execute chain and get `text` context variable
        Console.WriteLine(result);
    }
}
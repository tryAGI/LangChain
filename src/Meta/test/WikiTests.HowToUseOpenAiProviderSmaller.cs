using LangChain.Providers.OpenAI.Predefined;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task HowToUseOpenAiProviderSmaller()
    {
        var model = new OpenAiLatestFastChatModel("your_openAI_key");
        var chain =
            Set("Hello!")
            | LLM(model);

        Console.WriteLine(await chain.RunAsync("text"));
    }
}
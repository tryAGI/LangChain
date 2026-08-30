using Microsoft.Extensions.AI;
using OpenAI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task HowToUseOpenAiProviderSmaller()
    {
        IChatClient model = new OpenAIClient("your_openAI_key").GetChatClient("gpt-4o-mini").AsIChatClient();
        var chain =
            Set("Hello!")
            | LLM(model);

        Console.WriteLine(await chain.RunAsync("text"));
    }
}

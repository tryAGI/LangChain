using Microsoft.Extensions.AI;
using tryAGI.OpenAI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class OpenAiTests
{
    [Test]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();

        Tiktoken.ModelToEncoder.For(CreateChatCompletionRequestModel.Gpt4.ToValueString()).CountTokens(text).Should().Be(4300);
    }

    [Test]
    [Explicit("Requires OPENAI_API_KEY")]
    public async Task TestChat()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") is { Length: > 0 } key ? key :
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");

        var openAiClient = new OpenAI.OpenAIClient(apiKey);
        Microsoft.Extensions.AI.IChatClient chatClient = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();

        var chain =
            Set("Say hello in one word.", "prompt")
            | LLM(chatClient, inputKey: "prompt");

        await chain.RunAsync();
    }
}

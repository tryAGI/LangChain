using Microsoft.Extensions.AI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    [Explicit("Requires OPENAI_API_KEY")]
    public async Task GettingStarted()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") is { Length: > 0 } key ? key :
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");

        // Using OpenAI via MEAI IChatClient
        var openAiClient = new OpenAI.OpenAIClient(apiKey);
        IChatClient chatClient = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();

        // building a chain
        var prompt = @"
You are an AI assistant that greets the world.
World: Hello, Assistant!
Assistant:";

        var chain =
            Set(prompt, outputKey: "prompt")
            | LLM(chatClient, inputKey: "prompt");

        await chain.RunAsync();
    }
}

using Microsoft.Extensions.AI;

namespace LangChain.IntegrationTests;

public partial class WikiTests
{
    [Test]
    [Explicit("Requires ANTHROPIC_API_KEY")]
    public async Task GettingStartedWithAnthropic()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") is { Length: > 0 } key ? key :
            throw new InconclusiveException("ANTHROPIC_API_KEY environment variable is not found.");

        using var client = new Anthropic.AnthropicClient(apiKey);
        IChatClient chatClient = client;

        var prompt = @"
            you are a comic book writer. you will be given a question and you will answer it.
            question: who are 10 of the most popular superheros and what are their powers?";

        var response = await chatClient.GetResponseAsync(
            [new ChatMessage(ChatRole.User, prompt)],
            new ChatOptions { ModelId = "claude-sonnet-4-20250514" });

        Console.WriteLine(response.Text);
    }
}

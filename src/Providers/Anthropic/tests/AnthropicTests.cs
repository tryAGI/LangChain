
using LangChain.Providers.Anthropic;
using LangChain.Providers.Anthropic.Predefined;

namespace LangChain.Providers.OpenRouter.Tests;

[TestFixture, Explicit]
public class AnthropicTests
{
    [Test]
    public async Task ShouldGenerateFine_WithPredefinedModel()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ??
            throw new InvalidOperationException("ANTHROPIC_API_KEY is not set");

        var model = new Claude3Haiku(new AnthropicProvider(apiKey));

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
    }
}
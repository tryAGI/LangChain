using LangChain.Providers.OpenRouter.Predefined;

namespace LangChain.Providers.OpenRouter.Tests;

[TestFixture, Explicit]
public class OpenRouterTests
{
    [Test]
    public async Task ShouldGenerateFine_WithPredefinedModel()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Open_Router_Key") ??
            throw new InvalidOperationException("Open_Router_Key is not set");
        
        var model = new OpenAiGpt35TurboModel(new OpenRouterProvider(apiKey));

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task ShouldGenerateFine_With_Enum_Model()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Open_Router_Key", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("Open_Router_Key is not set");
        
        var model = new OpenRouterModel(new OpenRouterProvider(apiKey),OpenRouterModelIds.OpenAiGpt35Turbo);

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
    }
}
using LangChain.Providers.TogetherAi.Predefined;

namespace LangChain.Providers.TogetherAi.Tests;

[TestFixture, Explicit]
public class TogetherAiTests
{
    [Test]
    public async Task ShouldGenerateFine_WithPredefinedModel()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("TogetherAi_Api_Key",EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("TogetherAi_Api_Key is not set");
        
        var model = new Mixtral8X7BInstructV01Model(new TogetherAiProvider(apiKey));

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
        Console.WriteLine(result.LastMessageContent);
    }

    [Test]
    public async Task ShouldGenerateFine_With_Enum_Model()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("TogetherAi_Api_Key", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("TogetherAi_Api_Key is not set");
        
        var model = new TogetherAiModel(new TogetherAiProvider(apiKey),TogetherAiModelIds.OpenHermes25Mistral7B);

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
        Console.WriteLine(result.LastMessageContent);
    }
}
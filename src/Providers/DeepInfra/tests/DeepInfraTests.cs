using LangChain.Providers.DeepInfra.Predefined;

namespace LangChain.Providers.DeepInfra.Tests;

[TestFixture, Explicit]
public class DeepInfraTests
{
    [Test]
    public async Task ShouldGenerateFine_WithPredefinedModel()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Deep_Infra_Key") ??
            throw new InvalidOperationException("Deep_Infra_Key is not set");
        
        var model = new Wizardlm27BModel(new DeepInfraProvider(apiKey));

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task ShouldGenerateFine_With_Enum_Model()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Deep_Infra_Key", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("Deep_Infra_Key is not set");
        
        var model = new DeepInfraModel(new DeepInfraProvider(apiKey),DeepInfraModelIds.Wizardlm27B);

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
    }
}
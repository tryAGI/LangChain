using LangChain.Providers.DeepSeek.Predefined;

namespace LangChain.Providers.DeepSeek.Tests;

[TestFixture, Explicit]
public class DeepSeekTests
{
    [Test]
    public async Task ShouldGenerateFine_WithChatModel()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("DeepSeek_API_Key",EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("DeepSeek_API_Key is not set");
        
        var model = new DeepSeekChatModel(new DeepSeekProvider(apiKey));

        var result = await model.GenerateAsync("Write a Poem".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
        Console.WriteLine(result.LastMessageContent);
    }

    [Test]
    public async Task ShouldGenerateFine_With_CoderModel()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("DeepSeek_API_Key",EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("DeepSeek_API_Key is not set");
        
        var model = new DeepSeekCoderModel(new DeepSeekProvider(apiKey));

        var result = await model.GenerateAsync("Write a python script to count from 0 to 100".AsHumanMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
        Console.WriteLine(result.LastMessageContent);
    }
}
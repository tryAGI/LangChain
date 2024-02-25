using LangChain.Providers.Google.Predefined;

namespace LangChain.Providers.Google.UnitTests;

[TestFixture, Explicit]
public class GenerativeModelTests
{
    [Test]
    public async Task ShouldGenerateFine()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Gemini_API_Key", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("Gemini_API_Key is not set");
        var httpClient = new HttpClient();
        var provider = new GoogleProvider(apiKey, httpClient);
        var model = new GeminiProModel(provider);

        var result = await model.GenerateAsync("Write a Poem".AsChatMessage());

        result.Messages.Count.Should().BeGreaterThan(0);
        result.Messages.Last().Content.Should().NotBeNullOrEmpty();
    }
}
using LangChain.Providers.TogetherAi.Predefined;

namespace LangChain.Providers.TogetherAi.Tests;

[TestFixture]
[Explicit]
public class GeneralTests
{
    [Test]
    public async Task GetWeather()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("TogetherAi_Api_Key", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("TogetherAi_Api_Key environment variable is not found.");

        var model = new Mixtral8X7BInstructV01Model(new TogetherAiProvider(apiKey));

        var service = new WeatherService();
        model.AddGlobalTools(service.AsTools(), service.AsCalls());

        var response = await model.GenerateAsync(
            new[]
            {
                "You are a helpful weather assistant.".AsSystemMessage(),
                "What's the weather like today in Dubai, UAE?".AsHumanMessage()
            });

        Console.WriteLine(response.Messages.AsHistory());
    }
}
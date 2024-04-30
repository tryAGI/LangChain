
using LangChain.Providers.DeepInfra.Predefined;

namespace LangChain.Providers.DeepInfra.Tests;

[TestFixture, Explicit]
public class GeneralTests
{
    [Test]
    public async Task GetWeather()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Deep_Infra_Key") ??
            throw new InvalidOperationException("Deep_Infra_Key environment variable is not found.");

        var model = new Mixtral8X7BInstructV01Model(new DeepInfraProvider(apiKey));

        var service = new WeatherService();
        model.AddGlobalTools(service.AsTools(), service.AsCalls());

        var response = await model.GenerateAsync(
            new[]
            {
                 "You are a helpful weather assistant.".AsSystemMessage(),
                 "What's the weather like today?".AsHumanMessage(),
                 "Sure! Could you please provide me with your location?".AsAiMessage(),
                 "Dubai, UAE".AsHumanMessage(),
            });

        Console.WriteLine(response.Messages.AsHistory());
    }
}

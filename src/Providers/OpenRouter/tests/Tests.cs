
using LangChain.Providers.OpenRouter.Predefined;

namespace LangChain.Providers.OpenRouter.Tests;

[TestFixture, Explicit]
public class GeneralTests
{
    [Test]
    public async Task GetWeather()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Open_Router_Key") ??
            throw new InvalidOperationException("Open_Router_Key environment variable is not found.");
        
        var model = new OpenAiGpt35Turbo16KModel(new OpenRouterProvider(apiKey));

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

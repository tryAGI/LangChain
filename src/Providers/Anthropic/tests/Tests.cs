
//using LangChain.Providers.Anthropic.Extensions;
//using LangChain.Providers.Anthropic.Predefined;

//namespace LangChain.Providers.Anthropic.Tests;

//[TestFixture, Explicit]
//public class GeneralTests
//{
//    [Test]
//    public async Task GetWeather()
//    {
//        var apiKey =
//            Environment.GetEnvironmentVariable("Open_Router_Key") ??
//            throw new InvalidOperationException("Open_Router_Key environment variable is not found.");
        
//        var model = new Claude3Haiku(apiKey);

//        var service = new WeatherService();
//        model.AddGlobalTools(service.AsTools(), service.AsCalls());

//        var response = await model.GenerateAsync(
//            new[]
//            {
//                 "You are a helpful weather assistant.".AsSystemMessage(),
//                 "What's the weather like today?".AsHumanMessage(),
//                 "Sure! Could you please provide me with your location?".AsAiMessage(),
//                 "Dubai, UAE".AsHumanMessage(),
//            });

//        Console.WriteLine(response.Messages.AsHistory());
//    }
//}

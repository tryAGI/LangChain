namespace LangChain.Providers.OpenAI.IntegrationTests;

[TestFixture, Explicit]
public class GeneralTests
{
//     [TestMethod]
//     public async Task GetWeather()
//     {
//         var apiKey =
//             Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
//             throw new AssertInconclusiveException("OPENAI_API_KEY environment variable is not found.");
//
//         using var client = new HttpClient();
//         var model = new Gpt35TurboModel(apiKey, client);
//
//         var service = new WeatherService();
//         model.AddGlobalFunctions(service.AsFunctions(), service.AsCalls());
//
//         var response = await model.GenerateAsync(new ChatRequest(
//             Messages: new[]
//             {
//                 "You are a helpful weather assistant.".AsSystemMessage(),
//                 "What's the weather like today?".AsHumanMessage(),
//                 "Sure! Could you please provide me with your location?".AsAiMessage(),
//                 "Dubai, UAE".AsHumanMessage(),
//             }));
//
//         Console.WriteLine(response.Messages.AsHistory());
//     }

// //    [Test]
// //    public void SimpleTest()
// //    {
// //        var apiKey =
// //            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
// //            throw new AssertInconclusiveException("OPENAI_API_KEY environment variable is not found.");
// 
// //        var model = new Gpt35TurboModel(apiKey);
// 
// //        var prompt = "This is a test";
// 
// //        var response = model.GenerateAsync(new ChatRequest(new []{prompt.AsHumanMessage()})).Result;
// 
// //        Assert.IsTrue(true);
// //    }
}

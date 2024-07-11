// using LangChain.Providers.Reka.Predefined;
//
// namespace LangChain.Providers.Reka.Tests;
//
// [TestFixture]
// [Explicit]
// public class RekaTests
// {
//     [Test]
//     public async Task Chat()
//     {
//         using var httpClient = new HttpClient();
//         var provider = new RekaProvider(
//             apiKey: Environment.GetEnvironmentVariable("REKA_API_KEY") ??
//             throw new InconclusiveException("REKA_API_KEY is not set."),
//             httpClient);
//         var model = new FlashModel(provider);
//
//         string answer = await model.GenerateAsync("Generate some random name:");
//
//         answer.Should().NotBeNull();
//
//         Console.WriteLine(answer);
//     }
// }
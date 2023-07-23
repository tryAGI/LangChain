using LangChain.Providers;

var apiKey =
    Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
    throw new InvalidOperationException("OPENAI_API_KEY environment variable is not found.");
using var httpClient = new HttpClient();
var model = new Gpt35TurboModel(apiKey: apiKey, httpClient);

var result = await model.GenerateAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result);


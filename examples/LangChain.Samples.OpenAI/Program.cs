using LangChain.Providers.OpenAI.Predefined;

var apiKey =
    Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
    throw new InvalidOperationException("OPENAI_API_KEY environment variable is not found.");
var model = new Gpt35TurboModel(apiKey: apiKey);

var result = await model.GenerateAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result);


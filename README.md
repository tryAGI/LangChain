# 🦜️🔗 LangChain

[![Nuget package](https://img.shields.io/nuget/vpre/LangChain)](https://www.nuget.org/packages/LangChain/)
[![dotnet](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/github/license/tryAGI/LangChain)](https://github.com/tryAGI/LangChain/blob/main/LICENSE)
[![Discord](https://img.shields.io/discord/1115206893015662663?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discord.gg/Ca2xhfBf3v)

⚡ Building applications with LLMs through composability ⚡  
C# implementation of LangChain. We try to be as close to the original as possible in terms of abstractions, but are open to new entities.

While the [SemanticKernel](https://github.com/microsoft/semantic-kernel/) is good and we will use it wherever possible, we believe that it has many limitations and based on Microsoft technologies.
We proceed from the position of the maximum choice of available options and are open to using third-party libraries within individual implementations.  
❤️ Our project includes https://github.com/jeastham1993/langchain-dotnet and tries to be updated with the latest changes there ❤️  

## Usage
```csharp
var model = new Gpt4Model("API_KEY");
var response = await model.GenerateAsync("Hello, World of AI!");

var numberOfTokens = model.CountTokens("Hello, World of AI!");
```

### OpenAI Functions:
WeatherService:
```csharp
[OpenAiFunctions]
public interface IWeatherFunctions
{
    [Description("Get the current weather in a given location")]
    public Task<Weather> GetCurrentWeatherAsync(
        [Description("The city and state, e.g. San Francisco, CA")] string location,
        Unit unit = Unit.Celsius,
        CancellationToken cancellationToken = default);
}

public class WeatherService : IWeatherFunctions
{
    public Task<Weather> GetCurrentWeatherAsync(string location, Unit unit = Unit.Celsius, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Weather
        {
            Location = location,
            Temperature = 22.0,
            Unit = unit,
            Description = "Sunny",
        });
    }
}
```
```csharp
using var client = new HttpClient();
var model = new Gpt35TurboModel(apiKey, client);

var service = new WeatherService();
model.AddGlobalFunctions(service.AsFunctions(), service.AsCalls());

var response = await model.GenerateAsync(new ChatRequest(
    Messages: new []
    {
        "You are a helpful weather assistant.".AsSystemMessage(),
        "What's the weather like today?".AsHumanMessage(),
        "Sure! Could you please provide me with your location?".AsAiMessage(),
        "Dubai, UAE".AsHumanMessage(),
    }));

Console.WriteLine(response.Messages.AsHistory());
```
Result:
```
System: You are a helpful weather assistant.
Human: What's the weather like today?
AI: Sure! Could you please provide me with your location?
Human: Dubai, UAE
Function call: GetCurrentWeather({"location": "Dubai, UAE"})
Function result: GetCurrentWeather -> {"location":"Dubai, UAE","temperature":22,"unit":"celsius","description":"Sunny"}
AI: The weather in Dubai, UAE today is sunny with a temperature of 22 degrees Celsius.
```

## Support

Priority place for bugs: https://github.com/tryAGI/LangChain/issues  
Priority place for ideas and general questions: https://github.com/tryAGI/LangChain/discussions  
Discord: https://discord.gg/Ca2xhfBf3v  
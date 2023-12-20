# 🦜️🔗 LangChain

[![Nuget package](https://img.shields.io/nuget/vpre/LangChain)](https://www.nuget.org/packages/LangChain/)
[![dotnet](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/tryAGI/LangChain/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/github/license/tryAGI/LangChain)](https://github.com/tryAGI/LangChain/blob/main/LICENSE)
[![Discord](https://img.shields.io/discord/1115206893015662663?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discord.gg/Ca2xhfBf3v)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-5-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

⚡ Building applications with LLMs through composability ⚡  
C# implementation of LangChain. We try to be as close to the original as possible in terms of abstractions, but are open to new entities.

While the [SemanticKernel](https://github.com/microsoft/semantic-kernel/) is good and we will use it wherever possible, we believe that it has many limitations and based on Microsoft technologies.
We proceed from the position of the maximum choice of available options and are open to using third-party libraries within individual implementations.  
❤️ Our project includes https://github.com/jeastham1993/langchain-dotnet and tries to be updated with the latest changes there ❤️  

I want to note:
- I’m unlikely to be able to make serious progress alone, so my goal is to unite the efforts of C# developers to create a C# version of LangChain and control the quality of the final project
- I try to accept any Pull Request within 24 hours (of course, it depends, but I will try)
- I'm also looking for developers to join the core team. I will sponsor them whenever possible and also share any money received.
- I also respond quite quickly on Discord for any questions related to the project

## Usage
```csharp
const string apiKey = "API_KEY";
using var httpClient = new HttpClient();
var model = new Gpt4Model(apiKey, httpClient);
var response = await model.GenerateAsync("Hello, World of AI!");

var numberOfTokens = model.CountTokens("Hello, World of AI!");
```

### Chains
```csharp
const string apiKey = "API_KEY";
using var httpClient = new HttpClient();
var model = new Gpt4Model(apiKey, httpClient);

var template = "What is a good name for a company that makes {product}?";
var prompt = new PromptTemplate(new PromptTemplateInput(template, new List<string>(1){"product"}));

var chain = new LlmChain(new LlmChainInput(model, prompt));

var result = await chain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
{
    { "product", "colourful socks" }
}));
// The result is an object with a `text` property.
Console.WriteLine(result.Value["text"]);

// Since the LLMChain is a single-input, single-output chain, we can also call it with `run`.
// This takes in a string and returns the `text` property.
var result2 = await chain.Run("colourful socks");

Console.WriteLine(result2);
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

Also see [examples](./examples) for example usage.

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://www.upwork.com/freelancers/~017b1ad6f6af9cc189"><img src="https://avatars.githubusercontent.com/u/3002068?v=4?s=100" width="100px;" alt="Konstantin S."/><br /><sub><b>Konstantin S.</b></sub></a><br /><a href="#infra-HavenDV" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=HavenDV" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=HavenDV" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/TesAnti"><img src="https://avatars.githubusercontent.com/u/8780022?v=4?s=100" width="100px;" alt="TesAnti"/><br /><sub><b>TesAnti</b></sub></a><br /><a href="#infra-TesAnti" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=TesAnti" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=TesAnti" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/khoroshevj"><img src="https://avatars.githubusercontent.com/u/13628506?v=4?s=100" width="100px;" alt="Khoroshev Evgeniy"/><br /><sub><b>Khoroshev Evgeniy</b></sub></a><br /><a href="#infra-khoroshevj" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=khoroshevj" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=khoroshevj" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/SiegDuch"><img src="https://avatars.githubusercontent.com/u/104992451?v=4?s=100" width="100px;" alt="SiegDuch"/><br /><sub><b>SiegDuch</b></sub></a><br /><a href="#infra-SiegDuch" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/gunpal5"><img src="https://avatars.githubusercontent.com/u/10114874?v=4?s=100" width="100px;" alt="gunpal5"/><br /><sub><b>gunpal5</b></sub></a><br /><a href="#infra-gunpal5" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/tryAGI/LangChain/commits?author=gunpal5" title="Tests">⚠️</a> <a href="https://github.com/tryAGI/LangChain/commits?author=gunpal5" title="Code">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

## Support

Priority place for bugs: https://github.com/tryAGI/LangChain/issues  
Priority place for ideas and general questions: https://github.com/tryAGI/LangChain/discussions  
Discord: https://discord.gg/Ca2xhfBf3v  

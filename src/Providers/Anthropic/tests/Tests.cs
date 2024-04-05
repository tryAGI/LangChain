
using System.Xml.Serialization;
using LangChain.Providers.Anthropic.Extensions;
using LangChain.Providers.Anthropic.Predefined;
using LangChain.Providers.Anthropic.Tools;

namespace LangChain.Providers.Anthropic.Tests;

[TestFixture, Explicit]
public class GeneralTests
{
    [Test]
    public async Task GetBooks()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ??
            throw new InvalidOperationException("ANTHROPIC_API_KEY environment variable is not found.");

        var model = new Claude3Haiku(new AnthropicProvider(apiKey));

        var service = new BookStoreService();
        model.AddGlobalTools(service.AsAnthropicTools(), service.AsAnthropicCalls());
       
        var response = await model.GenerateAsync(
            new[]
            {
                 "what is written on page 35 in the book 'abracadabra'?".AsHumanMessage(),
            });

        Console.WriteLine(response.Messages.AsHistory());
    }
    [Test]
    public async Task GetWeather()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ??
            throw new InvalidOperationException("ANTHROPIC_API_KEY environment variable is not found.");

        var model = new Claude3Haiku(new AnthropicProvider(apiKey));

        var service = new WeatherService();
        model.AddGlobalTools(service.AsAnthropicTools(), service.AsAnthropicCalls());

        var response = await model.GenerateAsync(
            new[]
            {
                "How's the weather today in Nainital".AsHumanMessage(),
            });

        Console.WriteLine(response.Messages.AsHistory());
    }


    [Test]
    public async Task SerializeToolsToXml()
    {
        var anthropicTool = new AnthropicTool()
        {
            Description = "Sample Tool",
            Name = "Sample",
            Parameters =
            [
                new AnthropicToolParameter()
                    { Description = "Parameter Description", Name = "Name property", Type = "type property" }
            ]
        };

        AnthropicTools tools = new AnthropicTools();
        tools.Tools = new List<AnthropicTool>();
        tools.Tools.Add(anthropicTool);

        var xml = tools.ToXml();
    }
}

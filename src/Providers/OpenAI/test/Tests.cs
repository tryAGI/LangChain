using LangChain.Providers.OpenAI.Predefined;

namespace LangChain.Providers.OpenAI.Tests;

[TestFixture, Explicit]
public class GeneralTests
{
    [Test]
    public async Task GetWeather()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InvalidOperationException("OPENAI_API_KEY environment variable is not found.");

        //using var client = new HttpClient();
        var model = new Gpt35TurboModel(apiKey);

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

    [Test]
    public async Task SimpleTest()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");

        var model = new Gpt35TurboModel(apiKey);
        model.PromptSent += (_, prompt) => Console.WriteLine($"Prompt: {prompt}");
        model.PartialResponseGenerated += (_, delta) => Console.Write(delta);
        model.CompletedResponseGenerated += (_, prompt) => Console.WriteLine($"Completed response: {prompt}");
        
        var response = await model.GenerateAsync("This is a test");

        response.LastMessageContent.Should().NotBeNull();
    }
    
    [Test]
    public async Task StreamingTest()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");

        var model = new Gpt35TurboModel(apiKey)
        {
            Settings = new OpenAiChatSettings
            {
                UseStreaming = true,
            }
        };
        model.PromptSent += (_, prompt) => Console.WriteLine($"Prompt: {prompt}");
        model.PartialResponseGenerated += (_, delta) => Console.WriteLine(delta);
        model.CompletedResponseGenerated += (_, prompt) => Console.WriteLine($"Completed response: {prompt}");
        
        var response = await model.GenerateAsync("This is a test");

        response.LastMessageContent.Should().NotBeNull();
    }
}

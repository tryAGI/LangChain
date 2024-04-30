using GenerativeAI.Models;

namespace LangChain.Providers.Google.Tests;

[TestFixture, Explicit]
public class GeneralTests
{
    [Test]
    public async Task GetWeather()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("Gemini_API_Key", EnvironmentVariableTarget.User) ??
            throw new InvalidOperationException("Gemini_API_Key is not set");


        var model = new GoogleChatModel(apiKey, GoogleAIModels.GeminiPro);

        var service = new WeatherService();
        model.AddGlobalTools(service.AsGoogleFunctions(), service.AsGoogleCalls());

        var response = await model.GenerateAsync(
            new[]
            {
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
            Environment.GetEnvironmentVariable("Gemini_API_Key", EnvironmentVariableTarget.User) ??
            throw new InconclusiveException("Gemini_API_Key environment variable is not found.");

        var model = new GoogleChatModel(apiKey, GoogleAIModels.GeminiPro);
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
            Environment.GetEnvironmentVariable("Gemini_API_Key", EnvironmentVariableTarget.User) ??
            throw new InconclusiveException("Gemini_API_Key environment variable is not found.");

        var model = new GoogleChatModel(new GoogleProvider(apiKey, new HttpClient())
        {
            ChatSettings = new ChatSettings()
            {
                UseStreaming = true
            }
        }, GoogleAIModels.GeminiPro);

        model.PromptSent += (_, prompt) => Console.WriteLine($"Prompt: {prompt}");
        model.PartialResponseGenerated += (_, delta) => Console.WriteLine(delta);
        model.CompletedResponseGenerated += (_, prompt) => Console.WriteLine($"Completed response: {prompt}");

        var response = await model.GenerateAsync("This is a test");

        response.LastMessageContent.Should().NotBeNull();
    }
}

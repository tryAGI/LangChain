using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;

namespace LangChain.IntegrationTests;

[TestFixture]
public class BaseTests
{
    [TestCase(ProviderType.OpenAi)]
    [TestCase(ProviderType.Anyscale)]
    [TestCase(ProviderType.Together)]
    //[TestCase(ProviderType.Fireworks)]
    public async Task FiveRandomWords(ProviderType providerType)
    {
        var (llm, _) = Helpers.GetModels(providerType);
        llm.PromptSent += (_, prompt) => Console.WriteLine($"Prompt: {prompt}");
        llm.PartialResponseGenerated += (_, delta) => Console.Write(delta);
        llm.CompletedResponseGenerated += (_, prompt) => Console.WriteLine($"Completed response: {prompt}");

        var answer = await llm.GenerateAsync(
            request: "Answer me five random words",
            cancellationToken: CancellationToken.None).ConfigureAwait(false);
        
        Console.WriteLine($"LLM answer: {answer}"); // The cloaked figure.
        Console.WriteLine($"LLM usage: {llm.Usage}");    // Print usage and price

        answer.LastMessageContent.Should().NotBeNull();
    }

    [TestCase(ProviderType.OpenAi)]
    //[TestCase(ProviderType.Anyscale)]
    //[TestCase(ProviderType.Together)]
    //[TestCase(ProviderType.Fireworks)]
    public async Task Streaming(ProviderType providerType)
    {
        var (llm, _) = Helpers.GetModels(providerType);
        llm.PromptSent += (_, prompt) => Console.WriteLine($"Prompt: {prompt}");
        llm.PartialResponseGenerated += (_, delta) => Console.WriteLine(delta);
        llm.CompletedResponseGenerated += (_, prompt) => Console.WriteLine($"Completed response: {prompt}");
        
        llm.Settings = new ChatSettings
        {
            UseStreaming = true,
        };

        var response = await llm.GenerateAsync("This is a test");

        response.LastMessageContent.Should().NotBeNull();
    }
    
    [TestCase(ProviderType.OpenAi)]
    //[TestCase(ProviderType.Anyscale)]
    //[TestCase(ProviderType.Together)]
    //[TestCase(ProviderType.Fireworks)]
    public async Task GetWeather(ProviderType providerType)
    {
        var (llm, _) = Helpers.GetModels(providerType);

        var service = new WeatherService();
        llm.AddGlobalTools(service.AsTools(), service.AsCalls());

        var response = await llm.GenerateAsync(
            new[]
            {
                 "You are a helpful weather assistant.".AsSystemMessage(),
                 "What is the current temperature in Dubai, UAE in Celsius?".AsHumanMessage(),
            });

        Console.WriteLine(response.Messages.AsHistory());
    }

}
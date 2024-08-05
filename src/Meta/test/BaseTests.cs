using LangChain.Chains.LLM;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Schema;

namespace LangChain.IntegrationTests;

[TestFixture]
public class BaseTests
{
    [TestCase(ProviderType.OpenAi)]
    [TestCase(ProviderType.Anyscale)]
    [TestCase(ProviderType.Together)]
    [TestCase(ProviderType.OpenRouter)]
    [TestCase(ProviderType.DeepInfra)]
    //[TestCase(ProviderType.Fireworks)]
    //[TestCase(ProviderType.Google)]
    //[TestCase(ProviderType.Anthropic)]
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
    //[TestCase(ProviderType.OpenRouter)]
    //[TestCase(ProviderType.DeepInfra)]
    //[TestCase(ProviderType.Google)]
    //[TestCase(ProviderType.Anthropic)]
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
    //[TestCase(ProviderType.OpenRouter)]
    //[TestCase(ProviderType.DeepInfra)]
    //[TestCase(ProviderType.Google)]
    //[TestCase(ProviderType.Anthropic)]
    public async Task SimpleChain(ProviderType providerType)
    {
        var (llm, _) = Helpers.GetModels(providerType);

        const string template = "What is a good name for a company that makes {product}?";
        var prompt = new PromptTemplate(new PromptTemplateInput(template, ["product"]));

        var chain = new LlmChain(new LlmChainInput(llm, prompt));

        var result = await chain.CallAsync(new ChainValues(new Dictionary<string, object>(1)
        {
            ["product"] = "colourful socks",
        }));

        // The result is an object with a `text` property.
        result.Value["text"].ToString().Should().NotBeEmpty();

        Console.WriteLine(result.Value["text"]);
    }

    [TestCase(ProviderType.OpenAi)]
    //[TestCase(ProviderType.Anyscale)]
    [TestCase(ProviderType.Together)]
    [TestCase(ProviderType.OpenRouter)]
    //[TestCase(ProviderType.Fireworks)]
    //[TestCase(ProviderType.OpenRouter)]
    //[TestCase(ProviderType.DeepInfra)]
    //[TestCase(ProviderType.Google)]
    //[TestCase(ProviderType.Anthropic)]
    public async Task Tools_Weather(ProviderType providerType)
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
        response.Usage.InputTokens.Should().BeGreaterThan(0);
        response.Usage.OutputTokens.Should().BeGreaterThan(0);
        response.Usage.PriceInUsd.Should().BeGreaterThan(0);

        Console.WriteLine(response.Messages.AsHistory());
    }

    [TestCase(ProviderType.OpenAi)]
    //[TestCase(ProviderType.Anyscale)]
    //[TestCase(ProviderType.Together)]
    //[TestCase(ProviderType.OpenRouter)]
    //[TestCase(ProviderType.Fireworks)]
    //[TestCase(ProviderType.OpenRouter)]
    //[TestCase(ProviderType.DeepInfra)]
    //[TestCase(ProviderType.Google)]
    //[TestCase(ProviderType.Anthropic)]
    public async Task Tools_Books(ProviderType providerType)
    {
        var (llm, _) = Helpers.GetModels(providerType);

        var service = new BookStoreService();
        llm.AddGlobalTools(service.AsTools(), service.AsCalls());

        var response = await llm.GenerateAsync(
            new[]
            {
                "what is written on page 35 in the book 'abracadabra'?".AsHumanMessage(),
            });
        response.Usage.InputTokens.Should().BeGreaterThan(0);
        response.Usage.OutputTokens.Should().BeGreaterThan(0);
        response.Usage.PriceInUsd.Should().BeGreaterThan(0);

        Console.WriteLine(response.Messages.AsHistory());
    }
}
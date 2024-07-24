namespace LangChain.IntegrationTests;

[TestFixture]
public class BaseTests
{
    [TestCase(ProviderType.OpenAi)]
    [TestCase(ProviderType.Anyscale)]
    [TestCase(ProviderType.Together)]
    //[TestCase(ProviderType.Fireworks)]
    public async Task BaseTest(ProviderType providerType)
    {
        var (llm, _) = Helpers.GetModels(providerType);

        var answer = await llm.GenerateAsync(
            request: "Answer me five random words",
            cancellationToken: CancellationToken.None).ConfigureAwait(false);

        Console.WriteLine($"LLM answer: {answer}"); // The cloaked figure.
        Console.WriteLine($"LLM usage: {llm.Usage}");    // Print usage and price
    }
}
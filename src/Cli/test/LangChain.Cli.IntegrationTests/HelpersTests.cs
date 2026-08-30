using LangChain.Cli.Models;

namespace LangChain.Cli.IntegrationTests;

[TestFixture]
public class HelpersTests
{
    [Test]
    public void Requesty_UsesExpectedEndpointAndApiKeyEnvironmentVariable()
    {
        Helpers.GetEndpoint(Provider.Requesty).Should().Be(new Uri("https://router.requesty.ai/v1"));
        Helpers.GetApiKeyEnvironmentVariable(Provider.Requesty).Should().Be("REQUESTY_API_KEY");
    }
}

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

    [Test]
    public void Opper_UsesExpectedEndpointAndApiKeyEnvironmentVariable()
    {
        Helpers.GetEndpoint(Provider.Opper).Should().Be(new Uri("https://api.opper.ai/v3/compat"));
        Helpers.GetApiKeyEnvironmentVariable(Provider.Opper).Should().Be("OPPER_API_KEY");
    }
}

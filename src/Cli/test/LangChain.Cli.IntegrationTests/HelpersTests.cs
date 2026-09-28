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
    public void ApiRoute_UsesExpectedEndpointAndApiKeyEnvironmentVariable()
    {
        Helpers.GetEndpoint(Provider.ApiRoute).Should().Be(new Uri("https://global.api-route.com/v1"));
        Helpers.GetApiKeyEnvironmentVariable(Provider.ApiRoute).Should().Be("API_ROUTE_API_KEY");
    }
}

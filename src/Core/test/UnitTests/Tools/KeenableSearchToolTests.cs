using System.Net;
using System.Net.Http;
using System.Text;
using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;

namespace LangChain.Core.UnitTests.Tools;

[TestFixture]
public class KeenableSearchToolTests
{
    private sealed class RecordingHandler(string json) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
        }
    }

    private const string Response = """
        {"query":"cats","results":[
          {"title":"Cat","url":"https://en.wikipedia.org/wiki/Cat","snippet":"The cat is a small carnivorous mammal."},
          {"title":"Cats 101","url":"https://example.com/cats","snippet":"All about cats."}
        ]}
        """;

    [Test]
    public async Task KeylessRequestUsesPublicEndpointAndTitleHeader()
    {
        var handler = new RecordingHandler(Response);
        using var client = new HttpClient(handler);
        var tool = new KeenableSearchTool(apiKey: "", useCache: false, resultsLimit: 1, httpClient: client);

        var result = await tool.ToolTask("cats");

        handler.Request!.RequestUri!.ToString().Should().Be("https://api.keenable.ai/v1/search/public");
        handler.Request.Headers.GetValues("X-Keenable-Title").Should().ContainSingle().Which.Should().Be("LangChain .NET");
        handler.Request.Headers.Contains("X-API-Key").Should().BeFalse();
        handler.Body.Should().Contain("\"query\":\"cats\"").And.Contain("\"max_results\":1");
        result.Should().Contain("Cat").And.Contain("Source url: https://en.wikipedia.org/wiki/Cat");
        result.Should().NotContain("Cats 101");
    }

    [Test]
    public async Task KeyedRequestUsesSearchEndpointAndApiKeyHeader()
    {
        var handler = new RecordingHandler(Response);
        using var client = new HttpClient(handler);
        var tool = new KeenableSearchTool(apiKey: "test-key", useCache: false, httpClient: client);

        var result = await tool.ToolTask("cats");

        handler.Request!.RequestUri!.ToString().Should().Be("https://api.keenable.ai/v1/search");
        handler.Request.Headers.GetValues("X-API-Key").Should().ContainSingle().Which.Should().Be("test-key");
        result.Should().Contain("Cats 101");
    }

    [Test]
    public async Task EmptyResults()
    {
        using var client = new HttpClient(new RecordingHandler("""{"query":"x","results":[]}"""));
        var tool = new KeenableSearchTool(apiKey: "", useCache: false, httpClient: client);

        var result = await tool.ToolTask("x");

        result.Should().Contain("No results found");
    }

    [Test, Explicit]
    public async Task SearchCat()
    {
        var tool = new KeenableSearchTool(useCache: false, resultsLimit: 3);

        var result = await tool.ToolTask("find me a cat");

        Console.WriteLine(result);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Source url: http");
    }
}

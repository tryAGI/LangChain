using System.Net;
using System.Net.Sockets;
using System.Text;
using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;

namespace LangChain.Core.UnitTests.Tools;

[TestFixture, Explicit]
public class GoogleCustomSearchToolTests
{
    [Test]
    public async Task SearchCat()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("GOOGLE_SEARCH_API_KEY") ??
            throw new InvalidOperationException("GOOGLE_SEARCH_API_KEY is not set");
        var cx =
            Environment.GetEnvironmentVariable("GOOGLE_SEARCH_CX") ??
            throw new InvalidOperationException("GOOGLE_SEARCH_CX is not set");
        var tool = new GoogleCustomSearchTool(apiKey, cx, useCache: false, resultsLimit: 3);

        var result = await tool.ToolTask("find me a cat");

        Console.WriteLine(result);

        result.Should().NotBeNullOrEmpty();
    }
}

[TestFixture]
public class GoogleCustomSearchToolBaseAddressTests
{
    private const string ResponseJson =
        """{"items":[{"title":"Cat","link":"https://example.com/cat","snippet":"A small cat."}]}""";

    [TestCase("")]
    [TestCase("/google")]
    [TestCase("/google/")]
    public async Task SendsRequestToBaseAddress(string prefix)
    {
        var port = GetFreePort();
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Start();

        var requestTask = Task.Run(async () =>
        {
            var context = await listener.GetContextAsync().ConfigureAwait(false);
            var path = context.Request.Url!.AbsolutePath;
            var query = context.Request.Url!.Query;
            var body = Encoding.UTF8.GetBytes(ResponseJson);
            context.Response.ContentType = "application/json";
            await context.Response.OutputStream.WriteAsync(body).ConfigureAwait(false);
            context.Response.Close();
            return (path, query);
        });

        var tool = new GoogleCustomSearchTool(
            "test-key",
            "test-cx",
            useCache: false,
            baseAddress: $"http://127.0.0.1:{port}{prefix}");

        var result = await tool.ToolTask("cat");
        var (path, query) = await requestTask;

        path.Should().Be($"{prefix.TrimEnd('/')}/customsearch/v1");
        query.Should().Contain("key=test-key").And.Contain("cx=test-cx").And.Contain("q=cat");
        result.Should().Contain("Cat").And.Contain("Source url: https://example.com/cat");
    }

    private static int GetFreePort()
    {
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port;
    }
}

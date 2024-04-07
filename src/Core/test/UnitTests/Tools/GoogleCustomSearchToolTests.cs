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
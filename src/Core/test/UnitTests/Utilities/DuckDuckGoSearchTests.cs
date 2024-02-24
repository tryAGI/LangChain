using LangChain.Retrievers;
using LangChain.Utilities;

namespace LangChain.Core.UnitTests.Utilities;

[TestFixture]
public class DuckDuckGoSearchTests
{
    [Test]
    public async Task Run_Ok()
    {
        var search = new DuckDuckGoSearchApiWrapper();

        var result = await search.RunAsync("wikipedia");

        result.Should().NotBeEmpty();
        //result.Should().Contain("encyclopedia");
    }

    [Test]
    public async Task GetSnippets_Ok()
    {
        var search = new DuckDuckGoSearchApiWrapper();

        var result = await search.GetSnippetsAsync("wikipedia");

        result.Should().NotBeEmpty();
        //result.Should().Contain(v => v.Contains("encyclopedia"));
    }

    [Test]
    public async Task Retriever_Ok()
    {
        var search = new DuckDuckGoSearchApiWrapper();
        var retriever = new WebSearchRetriever(search);

        var result = await retriever.GetRelevantDocumentsAsync("wikipedia");

        result.Should().NotBeEmpty();
        //result.Should().Contain(d => d.PageContent.Contains("encyclopedia"));
    }
}
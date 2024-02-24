namespace LangChain.Sources.WebBase.IntegrationTests;

[TestFixture]
public class WebBaseSourceTests
{
    [Test]
    public async Task CheckText()
    {
        var loader = new WebBaseSource
        {
            Url = "https://en.wikipedia.org/wiki/Web_scraping"
        };

        var documents = await loader.LoadAsync();

        documents.Should().NotBeEmpty();
        var first = documents.First();

        first.PageContent.Should().Contain("Web scraping, web harvesting, or web data extraction is");
        first.PageContent.Should().Contain("This page was last edited on");
    }
}
namespace LangChain.Sources.WebBase.IntegrationTests;

[TestClass]
public class WebBaseSourceTests
{
    [TestMethod]
    public async Task CheckText()
    {
        var loader = new WebBaseSource
        {
            Url = "https://en.wikipedia.org/wiki/Web_scraping"
        };

        var documents = await loader.LoadAsync();

        documents.Should().NotBeEmpty();
        var first = documents.First();

        first.Content.Should().Contain("Web scraping, web harvesting, or web data extraction is");
        first.Content.Should().Contain("This page was last edited on");
    }
}
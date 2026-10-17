namespace LangChain.DocumentLoaders.IntegrationTests;

public partial class Tests
{
    [Test]
    public void Html_MathMlIntegrationPoint_UsesHtmlParsing()
    {
        // GHSA-pgww-w46g-26qg: HTML inside annotation-xml must not stay in MathML.
        var parser = new AngleSharp.Html.Parser.HtmlParser();
        using var document = parser.ParseDocument(
            "<math><annotation-xml encoding=\"text/html\"><title><a encoding=\"</title><img src=x onerror=alert()>\"></annotation-xml></math>");

        var image = document.QuerySelector("img");
        image.Should().NotBeNull();
        image!.NamespaceUri.Should().Be("http://www.w3.org/1999/xhtml");
    }

    [Test]
    public async Task Html()
    {
        var loader = new HtmlLoader();

        var documents = await loader.LoadAsync(DataSource.FromUrl("https://en.wikipedia.org/wiki/Web_scraping"));

        documents.Should().NotBeEmpty();
        var first = documents.First();

        first.PageContent.Should().Contain("Web scraping, web harvesting, or web data extraction is");
        first.PageContent.Should().Contain("This page was last edited on");
    }
}
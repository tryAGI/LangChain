﻿namespace LangChain.DocumentLoaders.IntegrationTests;

public partial class Tests
{
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
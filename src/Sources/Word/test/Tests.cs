namespace LangChain.Sources;

[TestFixture]
public class Tests
{
    [Test]
    public async Task LoadDocumentsUsingLegacyWay()
    {
        var documents = await WordSource.LoadDocumentsFromStreamAsync(H.Resources.filesample_1MB_docx.AsStream());

        documents.Should().NotBeEmpty();

        // check text from paragraph 1
        documents.First().PageContent.Should().Contain("Lorem ipsum ");

        // check text from paragraph 2
        documents.Skip(1).First().PageContent.Should().Contain("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc ac faucibus odio. ");
    }

    [Test]
    public async Task LoadDocumentsUsingNet6AndGreaterWay()
    {
        var documents = await H.Resources.filesample_1MB_docx.AsStream().LoadDocumentsAsync<WordSource>();

        documents.Should().NotBeEmpty();

        // check text from paragraph 1
        documents.First().PageContent.Should().Contain("Lorem ipsum ");

        // check text from paragraph 2
        documents.Skip(1).First().PageContent.Should().Contain("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc ac faucibus odio. ");
    }
}
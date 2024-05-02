using LangChain.Sources;

namespace LangChain.Core.UnitTests;

[TestFixture]
public class DocumentLoaderTests
{
    [Test]
    public async Task TextLoaderTest()
    {
        var filepath = Path.Combine(@"Resources", "state_of_the_union.txt");
        var loader = new FileLoader();
        var documents = await loader.LoadAsync(DataSource.FromPath(filepath));

        documents.Count.Should().Be(1);
        documents.First().PageContent.Should().NotBeNullOrEmpty();
        documents.First().Metadata.Should().NotBeNull();
        // documents.First().Metadata.Should().ContainKey("source");
        // documents.First().Metadata["source"].Should().Be(filepath);
        documents.First().Paragraphs().Should().NotBeEmpty();
        documents.First().Paragraphs().First().Should().NotBeNullOrWhiteSpace();
        documents.First().Paragraphs().Should().HaveCount(359);
    }
}
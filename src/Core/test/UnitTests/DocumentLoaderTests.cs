using LangChain.DocumentLoaders;

namespace LangChain.Core.UnitTests;

[TestFixture]
public class DocumentLoaderTests
{
    [Test]
    public void TextLoaderTest()
    {
        var filepath = Path.Combine(@"Resources", "state_of_the_union.txt");
        var loader = new TextLoader(filepath);
        var documents = loader.Load();
        
        documents.Count.Should().Be(1);
    }
}
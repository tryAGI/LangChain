using LangChain.DocumentLoaders;

namespace LangChain.Core.UnitTests;

[TestClass]
public class DocumentLoadesTests
{
    [TestMethod]
    public void TextLoaderTest()
    {
        var filepath = Path.Combine(@"Resources", "state_of_the_union.txt");
        var loader = new TextLoader(filepath);
        var documents = loader.Load();
        Assert.AreEqual(1, documents.Count);

    }
}
using LangChain.Providers;

namespace LangChain.Splitters.CSharp.UnitTests;

[TestClass]
public class CSharpSplitterTests
{
    [TestMethod]
    public void CountTokens()
    {
        var model = new Gpt4Model(apiKey: string.Empty);
        var text = H.Resources.SocketIoClient_cs.AsString();

        model.CountTokens(text).Should().Be(10069);
    }
}
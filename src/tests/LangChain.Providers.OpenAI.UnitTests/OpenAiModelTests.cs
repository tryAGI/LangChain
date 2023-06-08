using LangChain.Providers;

namespace LangChain.Splitters.CSharp.UnitTests;

[TestClass]
public class CSharpSplitterTests
{
    [TestMethod]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();

        OpenAiModelHelpers.CountTokens(modelId: "text-davinci-003", text).Should().Be(5904);
        OpenAiModelHelpers.CountTokens(modelId: "gpt-3.5-turbo", text).Should().Be(4300);
        OpenAiModelHelpers.CountTokens(modelId: "gpt-4", text).Should().Be(4300);
    }
}
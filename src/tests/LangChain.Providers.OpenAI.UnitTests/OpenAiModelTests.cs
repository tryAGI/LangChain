namespace LangChain.Providers.OpenAI.UnitTests;

[TestClass]
public class OpenAiModelTests
{
    [TestMethod]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();

        OpenAiModelHelpers.CountTokens(modelId: "text-davinci-003", text).Should().Be(5904);
        new Gpt35TurboModel(apiKey: "random").CountTokens(text).Should().Be(4300);
        new Gpt4Model(apiKey: "random").CountTokens(text).Should().Be(4300);
    }
}
namespace LangChain.Providers.OpenAI.UnitTests;

[TestClass]
public class OpenAiModelTests
{
    [TestMethod]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();
        using var httpClient = new HttpClient();
        
        Tiktoken.Encoding.ForModel("text-davinci-003").CountTokens(text).Should().Be(5904);
        new Gpt35TurboModel(apiKey: "random", httpClient).CountTokens(text).Should().Be(4300);
        new Gpt4Model(apiKey: "random", httpClient).CountTokens(text).Should().Be(4300);
    }
}
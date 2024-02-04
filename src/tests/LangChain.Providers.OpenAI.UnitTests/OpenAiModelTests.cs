using LangChain.Providers.OpenAI.Predefined;

namespace LangChain.Providers.OpenAI.UnitTests;

[TestFixture]
public class OpenAiModelTests
{
    [Test]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();

        Tiktoken.Encoding.ForModel("text-davinci-003").CountTokens(text).Should().Be(5904);
        new Gpt35TurboModel(apiKey: "sk-random").CountTokens(text).Should().Be(4300);
        new Gpt4Model(apiKey: "sk-random").CountTokens(text).Should().Be(4300);
    }
}
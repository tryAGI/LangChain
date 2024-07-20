using LangChain.Providers.OpenAI.Predefined;

namespace LangChain.Providers.OpenAI.Tests;

[TestFixture]
public class OpenAiModelTests
{
    [Test]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();

        Tiktoken.ModelToEncoder.For("text-davinci-003").CountTokens(text).Should().Be(5904);
        new Gpt35TurboModel("sk-random").CountTokens(text).Should().Be(4300);
        new Gpt4Model("sk-random").CountTokens(text).Should().Be(4300);
    }
}
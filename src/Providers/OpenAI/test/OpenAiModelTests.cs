using LangChain.Providers.OpenAI.Predefined;
using OpenAI;

namespace LangChain.Providers.OpenAI.Tests;

[TestFixture]
public class OpenAiModelTests
{
    [Test]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();

        Tiktoken.ModelToEncoder.For(CreateChatCompletionRequestModel.Gpt4.ToValueString()).CountTokens(text).Should().Be(4300);
        new Gpt35TurboModel("sk-random").CountTokens(text).Should().Be(4300);
        new Gpt4Model("sk-random").CountTokens(text).Should().Be(4300);
    }
}
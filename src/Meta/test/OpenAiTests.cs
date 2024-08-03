using LangChain.Chains.LLM;
using LangChain.Prompts;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.Schema;
using OpenAI;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
[Explicit]
public class OpenAiTests
{
    [Test]
    public void CountTokens()
    {
        var text = H.Resources.SocketIoClient_cs.AsString();

        Tiktoken.ModelToEncoder.For(CreateChatCompletionRequestModel.Gpt4.ToValueString()).CountTokens(text).Should().Be(4300);
        new Gpt4OmniMiniModel("sk-random").CountTokens(text).Should().Be(4300);
        new Gpt4Model("sk-random").CountTokens(text).Should().Be(4300);
    }
    
    [Test]
    public async Task TestAudio()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");
        var tts = new Tts1Model(apiKey);
        var stt = new Whisper1Model(apiKey);

        const string messageText = "My name is Jimmy.";
        var chain =
            Set(messageText, "message")
            | TTS(tts, inputKey: "message", outputKey: "audio").UseCache()
            | STT(stt, inputKey: "audio", outputKey: "text").UseCache();

        var result = await chain.RunAsync();
        var text = result.Value["text"].ToString() ?? string.Empty;

        messageText.ToLowerInvariant().Should().Be(text.ToLowerInvariant());
    }
}
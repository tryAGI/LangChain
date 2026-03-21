using LangChain.Providers.OpenAI.Predefined;
using tryAGI.OpenAI;
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
    [Explicit("Whisper1Model does not implement ISpeechToTextClient (MEAI). TODO: Update when OpenAI STT supports MEAI.")]
    public async Task TestAudio()
    {
        var apiKey =
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
            throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");
        var tts = new Tts1Model(apiKey);

        // TODO: Whisper1Model implements ISpeechToTextModel (old provider interface), not ISpeechToTextClient (MEAI).
        // STT chain now requires ISpeechToTextClient. Update when LangChain.Providers.OpenAI adds MEAI support.
        // var stt = new Whisper1Model(apiKey);

        const string messageText = "My name is Jimmy.";
        var chain =
            Set(messageText, "message")
            | TTS(tts, inputKey: "message", outputKey: "audio").UseCache();
            // | STT(stt, inputKey: "audio", outputKey: "text").UseCache();

        var result = await chain.RunAsync();
        // var text = result.Value["text"].ToString() ?? string.Empty;
        // messageText.ToLowerInvariant().Should().Be(text.ToLowerInvariant());
        Console.WriteLine("TTS completed. STT skipped - requires ISpeechToTextClient implementation.");
    }
}
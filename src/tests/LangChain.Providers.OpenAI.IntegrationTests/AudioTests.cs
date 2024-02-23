using static LangChain.Chains.Chain;

namespace LangChain.Providers.OpenAI.IntegrationTests;

[TestFixture]
public class AudioTests
{
    [Test]
    [Explicit]
    public async Task TestAudio()
    {
        var apiKey =
                         Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                         throw new InconclusiveException("OPENAI_API_KEY environment variable is not found.");
        var tts = new Predefined.Tts1Model(apiKey);
        var stt = new Predefined.Whisper1Model(apiKey);

        const string messageText = "My name is Jimmy.";
        var chain =
            Set(messageText, "message")
            | TTS(tts, inputKey: "message", outputKey: "audio").UseCache()
            | STT(stt, inputKey: "audio", outputKey: "text").UseCache();

        var result = await chain.Run();
        var text = result.Value["text"].ToString() ?? string.Empty;

        messageText.ToLowerInvariant().Should().Be(text.ToLowerInvariant());
    }
}
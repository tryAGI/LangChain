using OpenAI.Audio;
using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.IntegrationTests;
using static LangChain.Chains.Chain;


[TestClass]
public class AudioTests
{
    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
         [Ignore]
#endif
    public void TestAudio()
    {
        var apiKey =
                         Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                         throw new AssertInconclusiveException("OPENAI_API_KEY environment variable is not found.");
       
        var model = new OpenAiModel(apiKey, "does not matter");
        var ttsSettings= OpenAiTextToSpeechSettings.Default;
        var sttSettings = OpenAiSpeechToTextSettings.Default;

        var messageText="My name is Jimmy.";

        var chain =
            Set(messageText, "message")
            | TTS(model, ttsSettings, inputKey: "message", outputKey: "audio")
            | STT(model, sttSettings, inputKey: "audio", outputKey: "text");

        var res = chain.Run().Result;

        var text = res.Value["text"].ToString();

        Assert.AreEqual(messageText.ToLower(),text.ToLower());
    }

}
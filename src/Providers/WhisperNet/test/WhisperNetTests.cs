using System.Diagnostics;
using Whisper.net.Ggml;
using static LangChain.Chains.Chain;
namespace LangChain.Providers.WhisperNet.Tests;

[TestFixture]
[Explicit]
public class WhisperNetTests
{
    [Test]
    public async Task TestWhisperSTT()
    {
        var modelPath =
           await  WhisperNetModelDownloader.DownloadModel(GgmlType.Base, QuantizationType.NoQuantization, false, "./whisper");
        var model = WhisperNetSpeechToTextModel.FromPath(modelPath, new ()
        {
            Language = "en",
            Prompt = "I am Kennedy"
        });

        var data = await File.ReadAllBytesAsync("Resources/kennedy.wav");

        var chain =
            Set(data, "audio")
            | STT(model);

        var result = await chain.Run<string>("text");
        result.Should().Contain("nation should commit");
    }
}
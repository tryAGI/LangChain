using LangChain.Providers;
using LangChain.Providers.HuggingFace.Downloader;
using LangChain.Providers.LLamaSharp;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task GettingStarted()
    {
        // get model path
        var modelPath = await HuggingFaceModelDownloader.GetModelAsync(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");

        // load model
        var model = LLamaSharpModelInstruction.FromPath(modelPath).UseConsoleForDebug();

        // building a chain
        var prompt = @"
You are an AI assistant that greets the world.
World: Hello, Assistant!
Assistant:";

        var chain =
            Set(prompt, outputKey: "prompt")
            | LLM(model, inputKey: "prompt");

        await chain.RunAsync();
    }
}
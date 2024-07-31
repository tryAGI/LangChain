using LangChain.Providers.HuggingFace.Downloader;
using static LangChain.Chains.Chain;
using static LangChain.Extensions.Docker.Chain;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public class DockerTests
{
    [Test]
    [Explicit]
    public async Task SimpleHelloWorldTest()
    {
        var modelPath = await HuggingFaceModelDownloader.GetModelAsync(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");
        var model = LLamaSharpModelInstruction.FromPath(modelPath);
        model.Configuration.AntiPrompts = new[] { "[END]" };

        var chain = Template(@"Generate a python program that prints ""Hello world"" do not explain or comment the code. I expect only generated code from you. Print [END] after you are done.

Generated code:", outputKey: "prompt")
                    | LLM(model, inputKey: "prompt", outputKey: "code")
                    | ExtractCode("code", "data")
                    | SaveIntoFile("main.py")
                    | RunCodeInDocker(attachVolume: "./", outputKey: "result");
        var result = await chain.RunAsync();

        result.Value["result"].ToString()?.Trim().Should().Be("Hello world");
    }

}
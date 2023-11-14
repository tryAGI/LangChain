using LangChain.Extensions.Docker;
using LangChain.Providers.Downloader;
using LangChain.Schema;
using static LangChain.Chains.Chain;
using static LangChain.Extensions.Docker.Chain;
namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestClass]
public class DockerTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [TestMethod]
    public void SimpleHelloWorldTest()
    {
        var model = LLamaSharpModelInstruction.FromPath(ModelPath);

        var promptText =
            @"Generate a python program that prints ""Hello world"" do not explain or comment the code. I expect only generated code from you.

Generated code:";

        var chain = Template(promptText, outputKey:"prompt")
                    | LLM(model, inputKey:"prompt", outputKey:"code")
                    | RunCodeInDocker(inputKey:"code", outputKey:"result");
        var res = chain.Run().Result;

        Assert.AreEqual("Hello world", res.Value["result"].ToString()?.Trim());
    }
    
}
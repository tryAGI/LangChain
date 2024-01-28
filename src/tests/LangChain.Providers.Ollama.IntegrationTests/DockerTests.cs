using System.Diagnostics;

namespace LangChain.Providers.Ollama.IntegrationTests;
using static LangChain.Chains.Chain;
using static LangChain.Extensions.Docker.Chain;
[TestFixture]
public class DockerTests
{
    [Test]
    [Explicit]
    public void SimpleHelloWorldTest()
    {
        var model = new OllamaLanguageModelInstruction("deepseek-coder:6.7b-instruct",options:new OllamaLanguageModelOptions()
        {
            Temperature = 0.0f,
            Stop = new[] { "[END]"}
        });
        

        var promptText =
            @"Generate a C# program that prints ""Hello, Anti!"". Do not explain the code. Print [END] after code is generated.
Code:
";

        Directory.CreateDirectory("test");
        CreateCSharpProject("test");


        var chain = Template(promptText, outputKey: "prompt")
                    | LLM(model, inputKey: "prompt", outputKey: "code")
                    | ExtractCode("code", "data")
                    | SaveIntoFile("test\\Program.cs")
                    | RunCodeInDocker(image: "mcr.microsoft.com/dotnet/sdk:8.0", command:"dotnet", arguments:"run",attachVolume: "./test", outputKey: "result");
        var res = chain.Run().Result;

        Assert.AreEqual("Hello, Anti!", res.Value["result"].ToString()?.Trim());
    }

    void CreateCSharpProject(string path)
    {
        Process.Start("dotnet", $"new console -o {path}").WaitForExit();
    }
}
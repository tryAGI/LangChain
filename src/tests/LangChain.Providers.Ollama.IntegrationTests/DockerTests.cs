using System.Diagnostics;
using static LangChain.Chains.Chain;
using static LangChain.Extensions.Docker.Chain;

namespace LangChain.Providers.Ollama.IntegrationTests;

[TestFixture]
public class DockerTests
{
    [Test]
    [Explicit]
    public async Task SimpleHelloWorldTest()
    {
        var model = new OllamaChatModel(new OllamaProvider(), id: "deepseek-coder:6.7b-instruct")
        {
            Settings = new ChatSettings
            {
                //Temperature = 0.0,
                StopSequences = ["[END]"],
            }
        };

        Directory.CreateDirectory("test");
        CreateCSharpProject("test");

        var chain = Template(@"Generate a C# program that prints ""Hello, Anti!"". Do not explain the code. Print [END] after code is generated.
Code:
", outputKey: "prompt")
                    | LLM(model, inputKey: "prompt", outputKey: "code")
                    | ExtractCode("code", "data")
                    | SaveIntoFile("test\\Program.cs")
                    | RunCodeInDocker(image: "mcr.microsoft.com/dotnet/sdk:8.0", command:"dotnet", arguments:"run",attachVolume: "./test", outputKey: "result");
        var result = await chain.Run();

        result.Value["result"].ToString()?.Trim().Should().Be("Hello, Anti!");
    }

    private static void CreateCSharpProject(string path)
    {
        Process.Start("dotnet", $"new console -o {path}").WaitForExit();
    }
}
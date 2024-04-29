using static LangChain.Chains.Chain;

namespace LangChain.Providers.Automatic1111.Tests;

[TestFixture]
public class Automatic1111Tests
{
    [Test]
    [Explicit]
    public async Task InstructionTest()
    {
        var model = new Automatic1111Model();
        var tempPath= Path.Combine(Path.GetTempPath(), "test.png");

        var chain =
            Set("Car", outputKey: "prompt")
            | GenerateImage(model, inputKey: "prompt", outputKey: "image")
            | SaveIntoFile(path: tempPath, inputKey: "image");
            
        await chain.RunAsync();
    }
}
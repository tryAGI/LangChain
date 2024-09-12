using LangChain.Providers.HuggingFace.Downloader;
using LangChain.Providers.LLamaSharp;
using static LangChain.Chains.Chain;

namespace LangChain.IntegrationTests;

[TestFixture]
public partial class WikiTests
{
    [Test]
    public async Task UsingChainOutput()
    {
        //// # Setup
        //// We will take the code from [Getting started] tutorial as our starting point.

        // get model path
        var modelPath = await HuggingFaceModelDownloader.GetModelAsync(
            repository: "TheBloke/Thespis-13B-v0.5-GGUF",
            fileName: "thespis-13b-v0.5.Q2_K.gguf",
            version: "main");

        // load model
        var model = LLamaSharpModelInstruction.FromPath(modelPath);

        // building a chain
        var prompt = @"
You are an AI assistant that greets the world.
World: Hello, Assistant!
Assistant:";

        //// # Getting the chain output
        //// 
        //// Almost every possible link in a chain are having having at least one input and output.
        //// 
        //// Look here:

        var chain =
            Set(prompt, outputKey: "prompt")
            | LLM(model, inputKey: "prompt", outputKey: "result");

        //// This means that, after link `Set` get executed, we are storring it's result into "prompt" variable inside of chain context.
        //// In its turn, link `LLM` gets "prompt" variable from chain context and uses it's as input.
        //// 
        //// `LLM` link also has output key argument. Let's use it to save the result of llm.

        var result = await chain.RunAsync("result");

        //// Now the `LLM` link saves it's result into "result" variable inside of chain context. But how do we extract it from there?
        //// 
        //// `chain.Run()` method has an optional argument "resultKey". This allows you to specify variable inside of chain context to return as a result.

        Console.WriteLine(result);

        //// Output:
        //// ```
        //// Hello, World! How can I help you today?
        //// ```
    }
}
using LangChain.Chains;
using LangChain.Databases.InMemory;
using LangChain.Docstore;
using LangChain.Providers.Downloader;
using static LangChain.Chains.Chain;
namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestClass]
public class ChainTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [TestMethod]
    public void PromptTest()
    {
        var chain =
            Set("World", outputKey: "var2")
            | Set("Hello", outputKey: "var1")
            | Template("{var1}, {var2}", outputKey: "prompt");

        var res = chain.Run(resultKey: "prompt").Result;

        Assert.AreEqual("Hello, World", res);
    }

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void LLMChainTest()
    {
        var llm = LLamaSharpModelInstruction.FromPath(ModelPath);
        var promptText =
            @"You will be provided with information about pet. Your goal is to extract the pet name.

Information:
{information}

The pet name is 
";

        var chain =
            Set("My dog name is Bob", outputKey: "information")
            | Template(promptText, outputKey: "prompt")
            | LLM(llm, inputKey: "prompt", outputKey: "text");

        var res = chain.Run(resultKey: "text").Result;

        Assert.AreEqual("Bob", res);
    }

    
}
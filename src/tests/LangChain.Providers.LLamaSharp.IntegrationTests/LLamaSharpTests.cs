using LangChain.Providers;
using LangChain.Providers.Downloader;
using LangChain.Providers.LLamaSharp;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestClass]
public class LLamaSharpTests
{
    string ModelPath=>HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf","main").Result;
    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void PrepromptTest()
    {
        var model = new LLamaSharpModelChat(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
        });

        var response=model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are simple assistant. If human say 'Bob' then you will respond with 'Jack'.".AsSystemMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
            "Jack".AsAiMessage(),
            "Bob".AsHumanMessage(),
        })).Result;

        Assert.AreEqual("Jack",response.Messages.Last().Content );

    }

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void InstructionTest()
    {
        var model = new LLamaSharpModelInstruction(new LLamaSharpConfiguration
        {
            PathToModelFile = ModelPath,
            Temperature=0
        });

        var response = model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are a calculator. Print the result of this expression: 2 + 2.".AsSystemMessage(),
            "Result:".AsSystemMessage(),
        })).Result;

        Assert.AreEqual("4",response.Messages.Last().Content.Trim());

    }
}
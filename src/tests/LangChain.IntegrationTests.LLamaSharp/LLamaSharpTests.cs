using LangChain.Providers;
using LangChain.Providers.LLamaSharp;

namespace LangChain.IntegrationTests;

[TestClass]
public class LLamaSharpTests
{
    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void PrepromptTest()
    {
        var model = new LLamaSharpModel(new LLamaSharpConfiguration
        {
            PathToModelFile = Path.Combine(Environment.ExpandEnvironmentVariables("%LLAMA_MODELS%"), "ggml-model-f32-q4_0.bin"),
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

        Assert.AreEqual(response.Messages.Last().Content, "Jack");

    }

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void InstructionTest()
    {
        var model = new LLamaSharpModel(new LLamaSharpConfiguration
        {
            PathToModelFile = Path.Combine(Environment.ExpandEnvironmentVariables("%LLAMA_MODELS%"), "ggml-model-f32-q4_0.bin"),
            Mode = ELLamaSharpModelMode.Instruction
        });

        var response=model.GenerateAsync(new ChatRequest(new List<Message>
        {
            "You are a calculator. You will be provided with expression. You must calculate it and print the result. Do not add any addition information.".AsSystemMessage(),
            "2 + 2".AsSystemMessage(),
        })).Result;

        Assert.IsTrue(response.Messages.Last().Content.Trim().Equals("4"));

    }
}
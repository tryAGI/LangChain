using LangChain.Memory;
using LangChain.Providers.Downloader;
using static LangChain.Chains.Chain;
namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestClass]
public class MessageHistoryTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [TestMethod]
#if CONTINUOUS_INTEGRATION_BUILD
    [Ignore]
#endif
    public void TestHistory()
    {
        var model = LLamaSharpModelInstruction.FromPath(ModelPath);

        var memory = new ConversationBufferMemory(new ChatMessageHistory());

        var promptText =
            @"You are a helpful chatbot
{chat_history}
Human: {message}
AI: ";

        var message = Set("hi, i am Jimmy", "message");

        var chain =
            message
            | Set(() => memory.BufferAsString, outputKey: "chat_history")
            | Template(promptText, outputKey: "prompt")
            | LLM(model, inputKey: "prompt", outputKey: "text")
            | UpdateMemory(memory, requestKey: "message", responseKey: "text");
                    
        chain.Run().Wait();

        message.Query = "what is my name?";

        var res=chain.Run().Result;

        Assert.AreEqual(4,memory.BufferAsMessages.Count);
        Assert.IsTrue(res.Value["text"].ToString()?.ToLower()?.Trim().Contains("jimmy"));

    }

}
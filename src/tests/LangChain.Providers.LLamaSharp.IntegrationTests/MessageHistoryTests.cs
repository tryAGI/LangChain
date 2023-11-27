using LangChain.Memory;
using LangChain.Providers.Downloader;
using static LangChain.Chains.Chain;
namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public class MessageHistoryTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [Test]
    [Explicit]
    public void TestHistory()
    {
        var model = LLamaSharpModelInstruction.FromPath(ModelPath);


        var promptText =
            @"You are a helpful chatbot
{chat_history}
Human: {message}
AI: ";

        var memory = new ConversationBufferMemory(new ChatMessageHistory());

        var message = Set("hi, i am Jimmy", "message");

        var chain =
            message
            | Set(() => memory.BufferAsString, outputKey: "chat_history") // get lates messages from buffer every time
            | Template(promptText, outputKey: "prompt")
            | LLM(model, inputKey: "prompt", outputKey: "text")
            | UpdateMemory(memory, requestKey: "message", responseKey: "text"); // save the messages to the buffer
                    
        chain.Run().Wait(); // call the chain for the first time.
                            // memory would contain 2 messages(1 from Human, 1 from AI).

        message.Value = "what is my name?"; // change the message.
                                            // This will appear as a new message from human

        var res=chain.Run().Result;  // call the chain for the second time.
                                                // prompt will contain previous messages and a question about the name.

        Assert.AreEqual(4,memory.BufferAsMessages.Count);
        Assert.IsTrue(res.Value["text"].ToString().ToLower()?.Trim().Contains("jimmy"));
    }

}
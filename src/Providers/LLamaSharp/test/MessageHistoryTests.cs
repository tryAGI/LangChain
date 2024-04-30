﻿using LangChain.Memory;
using LangChain.Providers.HuggingFace.Downloader;
using static LangChain.Chains.Chain;

namespace LangChain.Providers.LLamaSharp.IntegrationTests;

[TestFixture]
public class MessageHistoryTests
{
    string ModelPath => HuggingFaceModelDownloader.Instance.GetModel("TheBloke/Thespis-13B-v0.5-GGUF", "thespis-13b-v0.5.Q2_K.gguf", "main").Result;

    [Test]
    [Explicit]
    public async Task TestHistory()
    {
        var model = LLamaSharpModelInstruction.FromPath(ModelPath);


        var promptText =
            @"You are a helpful chatbot
{chat_history}
Human: {message}
AI: ";

        var history = new ChatMessageHistory();
        var memory = new ConversationBufferMemory(history);

        var message = Set("hi, i am Jimmy", "message");

        var chain =
            message
            | LoadMemory(memory, outputKey: "chat_history") // get lates messages from buffer every time
            | Template(promptText, outputKey: "prompt")
            | LLM(model, inputKey: "prompt", outputKey: "text")
            | UpdateMemory(memory, requestKey: "message", responseKey: "text"); // save the messages to the buffer

        await chain.RunAsync(); // call the chain for the first time.
                                // memory would contain 2 messages(1 from Human, 1 from AI).

        message.Value = "what is my name?"; // change the message.
                                            // This will appear as a new message from human

        var res = await chain.RunAsync();  // call the chain for the second time.
                                           // prompt will contain previous messages and a question about the name.

        history.Messages.Count.Should().Be(4);
        res.Value["text"].ToString()?.ToLower().Trim().Contains("jimmy").Should().BeTrue();
    }

}
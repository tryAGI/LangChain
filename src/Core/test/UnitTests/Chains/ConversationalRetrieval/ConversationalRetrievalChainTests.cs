using LangChain.Callback;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.ConversationalRetrieval;
using LangChain.Chains.LLM;
using LangChain.Sources;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Retrievers;
using LangChain.Schema;
using Moq;

namespace LangChain.Core.UnitTests.Chains.ConversationalRetrieval;

[TestFixture]
public class ConversationalRetrievalChainTests
{
    [Test]
    public async Task Call_Ok()
    {
        var combineDocumentsChainInput = new Mock<BaseCombineDocumentsChainInput>().Object;

        var combineDocsChainMock = new Mock<BaseCombineDocumentsChain>(combineDocumentsChainInput);
        combineDocsChainMock.Setup(x => x
                .RunAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<ICallbacks?>(), It.IsAny<CancellationToken>()))
            .Returns<Dictionary<string, object>, ICallbacks, CancellationToken>((_, _, _) => Task.FromResult("Alice"));

        var retrieverMock = new Mock<BaseRetriever>();
        retrieverMock
            .Setup(x => x
                .GetRelevantDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ICallbacks>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<CancellationToken>()))
            .Returns<string, string, ICallbacks, bool, List<string>, Dictionary<string, object>, CancellationToken>((_, _, _, _, _, _, _) =>
            {
                var docs = new List<Document>
                {
                    new("first"),
                    new("second"),
                    new("third")
                };

                return Task.FromResult<IReadOnlyCollection<Document>>(docs);
            });

        // # This controls how the standalone question is generated.
        // # Should take `chat_history` and `question` as input variables.
        var template =
            "Combine the chat history and follow up question into a standalone question. Chat History: {chat_history}. Follow up question: {question}";

        var prompt = PromptTemplate.FromTemplate(template);
        var questionGeneratorLlmMock = new Mock<IChatModel>();
        questionGeneratorLlmMock
            .Setup(v => v.GenerateAsync(It.IsAny<ChatRequest>(), It.IsAny<ChatSettings>(), It.IsAny<CancellationToken>()))
            .Returns<ChatRequest, ChatSettings, CancellationToken>((_, _, _) =>
            {
                return Task.FromResult(new ChatResponse
                {
                    Messages = new[] { Message.Ai("Bob's asking what is hist name") },
                    Usage = Usage.Empty,
                    UsedSettings = ChatSettings.Default,
                });
            });

        var llmInput = new LlmChainInput(questionGeneratorLlmMock.Object, prompt);
        var questionGeneratorChain = new LlmChain(llmInput);

        var chainInput = new ConversationalRetrievalChainInput(retrieverMock.Object, combineDocsChainMock.Object, questionGeneratorChain)
        {
            ReturnSourceDocuments = true,
            ReturnGeneratedQuestion = true
        };

        var chain = new ConversationalRetrievalChain(chainInput);

        var input = new ChainValues
        {
            ["question"] = "What is my name?",
            ["chat_history"] = new List<Message>
            {
                Message.Human("My name is Alice"),
                Message.Ai("Hello Alice")
            }
        };

        var result = await chain.CallAsync(input);

        result.Should().NotBeNull();
        result.Value.Should().ContainKey("answer");
        result.Value["answer"].Should().BeEquivalentTo("Alice");

        result.Value.Should().ContainKey("source_documents");
        var resultSourceDocuments = result.Value["source_documents"] as List<Document>;
        resultSourceDocuments.Should().NotBeNull();
        resultSourceDocuments.Should().HaveCount(3);
        resultSourceDocuments![0].PageContent.Should().BeEquivalentTo("first");

        result.Value.Should().ContainKey("generated_question");
        result.Value["generated_question"].Should().BeEquivalentTo("Bob's asking what is hist name");

        questionGeneratorLlmMock
            .Verify(v => v.GenerateAsync(
                It.Is<ChatRequest>(request => request.Messages.Count == 1),
                It.IsAny<ChatSettings>(),
                It.IsAny<CancellationToken>()));
    }

    [Test]
    public void ReduceTokensBelowLimit_Ok()
    {
        var supportsCountTokensMock = new Mock<ISupportsCountTokens>();
        supportsCountTokensMock
            .Setup(v => v.CountTokens(It.IsAny<string>()))
            .Returns<string>(input => input.Length);

        var chatModelMock = supportsCountTokensMock.As<IChatModel>();

        var llmWithCounterMock = new Mock<ILlmChain>();
        llmWithCounterMock
            .SetupGet(v => v.Llm)
            .Returns(chatModelMock.Object);

        var prompt = PromptTemplate.FromTemplate("{documents}");
        llmWithCounterMock
            .SetupGet(v => v.Prompt)
            .Returns(prompt);

        var combineDocsChainInput = new StuffDocumentsChainInput(llmWithCounterMock.Object);
        var combineDocsChain = new StuffDocumentsChain(combineDocsChainInput);

        var retriever = new Mock<BaseRetriever>().Object;
        var questionGeneratorChain = new Mock<ILlmChain>().Object;

        var chainInput = new ConversationalRetrievalChainInput(retriever, combineDocsChain, questionGeneratorChain)
        {
            MaxTokensLimit = 500
        };

        var chain = new ConversationalRetrievalChain(chainInput);

        var inputDocs = Enumerable.Range(1, 10).Select(_ => new Document(new string('*', 100)));
        var result = chain.ReduceTokensBelowLimit(inputDocs);

        result.Should().HaveCount(5);
    }
}
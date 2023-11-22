using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Docstore;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Schema;
using Moq;

namespace LangChain.Core.UnitTests.Chains.CombineDocuments;

[TestClass]
public class ReduceDocumentsChainTests
{
    [TestMethod]
    public async Task CollapseAndSummarize()
    {
        var document1 = CreateDocument("First page text. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.");
        var document2 = CreateDocument("Second page different text. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.");

        var combineLlmChain = CreateFakeLlmChain(
            _ => "summarized",
            PromptTemplate.FromTemplate("Summarize this content: {content}"));

        var combineDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(combineLlmChain.Object));

        var collapsed = new Dictionary<string, string>
        {
            [document1.PageContent] = "first collapsed",
            [document2.PageContent] = "second collapsed"
        };

        var collapseLlmChain = CreateFakeLlmChain(
            values => collapsed[values.Value["content"].ToString()],
            PromptTemplate.FromTemplate("Collapse this content: {content}"));

        var collapseDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(collapseLlmChain.Object));

        var input = new ReduceDocumentsChainInput
        {
            CombineDocumentsChain = combineDocumentsChain,
            CollapseDocumentsChain = collapseDocumentsChain,
            TokenMax = 100
        };

        var chain = new ReduceDocumentsChain(input);
        
        chain.InputKeys.Should().HaveCount(1);
        chain.InputKeys[0].Should().BeEquivalentTo("input_documents");

        chain.OutputKeys.Should().HaveCount(1);
        chain.OutputKeys[0].Should().BeEquivalentTo("output_text");

        chain.ChainType().Should().BeEquivalentTo("reduce_documents_chain");

        var result = await chain.CombineDocsAsync(new [] { document1, document2 }, new Dictionary<string, object>());

        result.Output.Should().BeEquivalentTo("summarized");
        result.OtherKeys.Should().BeEmpty();

        collapseLlmChain
            .Verify(
                m => m.Predict(
                    It.Is<ChainValues>(x => x.Value["content"].ToString() == document1.PageContent)),
                Times.Once());

        collapseLlmChain
            .Verify(
                m => m.Predict(
                    It.Is<ChainValues>(x => x.Value["content"].ToString() == document2.PageContent)),
                Times.Once());

        combineLlmChain
            .Verify(
                m => m.Predict(
                    It.Is<ChainValues>(x => x.Value["content"].ToString() == "first collapsed\n\nsecond collapsed")),
                Times.Once());
    }

    private Mock<ILlmChain> CreateFakeLlmChain(Func<ChainValues, string> predict, PromptTemplate promptTemplate)
    {
        var mock = new Mock<ILlmChain>();

        mock.Setup(x => x
                .Predict(It.IsAny<ChainValues>()))
            .Returns<ChainValues>(value => Task.FromResult((object)predict(value)));

        mock.Setup(x => x.Prompt)
            .Returns(promptTemplate);

        var supportsCountTokensMock = new Mock<ISupportsCountTokens>();
        supportsCountTokensMock
            .Setup(x => x.CountTokens(It.IsAny<string>()))
            .Returns<string>(text => text.Length);

        var chatModelMock = supportsCountTokensMock.As<IChatModel>();

        mock
            .Setup(x => x.Llm)
            .Returns(chatModelMock.Object);

        return mock;
    }

    private Document CreateDocument(string content) => new(content, new());

    private Document CreateDocument(string content, params (string Key, object Value)[] metadatas)
    {
        var metadata = metadatas.ToDictionary(kv => kv.Key, kv => kv.Value);

        return new Document(content, metadata);
    }
}
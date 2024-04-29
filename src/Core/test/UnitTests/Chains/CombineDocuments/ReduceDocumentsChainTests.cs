using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Sources;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Schema;
using Moq;

namespace LangChain.Core.UnitTests.Chains.CombineDocuments;

[TestFixture]
public class ReduceDocumentsChainTests
{
    [Test]
    public async Task CollapseAndSummarize()
    {
        var document1 = new Document("First page text. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.");
        var document2 = new Document("Second page different text. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.");

        var combineLlmChain = CreateFakeLlmChain(
            _ => "summarized",
            PromptTemplate.FromTemplate("Summarize this content: {content}"));

        var combineDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(combineLlmChain.Object));

        var collapsed = new Dictionary<string, string>
        {
            [document1.PageContent] = "first collapsed",
            [document2.PageContent] = "second collapsed",
        };

        var collapseLlmChain = CreateFakeLlmChain(
            values => collapsed[values.Value["content"].ToString() ?? ""],
            PromptTemplate.FromTemplate("Collapse this content: {content}"));

        var collapseDocumentsChain = new StuffDocumentsChain(new StuffDocumentsChainInput(collapseLlmChain.Object));

        var input = new ReduceDocumentsChainInput
        {
            CombineDocumentsChain = combineDocumentsChain,
            CollapseDocumentsChain = collapseDocumentsChain,
            TokenMax = 100,
        };

        var chain = new ReduceDocumentsChain(input);
        
        chain.InputKeys.Should().HaveCount(1);
        chain.InputKeys[0].Should().BeEquivalentTo("input_documents");

        chain.OutputKeys.Should().HaveCount(1);
        chain.OutputKeys[0].Should().BeEquivalentTo("output_text");

        chain.ChainType().Should().BeEquivalentTo("reduce_documents_chain");

        var result = await chain.CombineDocsAsync(new []
        {
            document1,
            document2,
        }, new Dictionary<string, object>());

        result.Output.Should().BeEquivalentTo("summarized");
        result.OtherKeys.Should().BeEmpty();

        AssertLlmChainPredictCalledWithContent(collapseLlmChain, "content", document1.PageContent);
        AssertLlmChainPredictCalledWithContent(collapseLlmChain, "content", document2.PageContent);

        AssertLlmChainPredictCalledWithContent(combineLlmChain, "content", "first collapsed\n\nsecond collapsed");
    }

    private void AssertLlmChainPredictCalledWithContent(Mock<ILlmChain> mock, string valueKey, string content)
    {
        mock
            .Verify(
                m => m.PredictAsync(
                    It.Is<ChainValues>(x => x.Value[valueKey].ToString() == content),
                    It.IsAny<CancellationToken>()),
                Times.Once());
    }

    private Mock<ILlmChain> CreateFakeLlmChain(
        Func<ChainValues, string> predict,
        PromptTemplate promptTemplate)
    {
        var mock = new Mock<ILlmChain>();

        mock.Setup(x => x
                .PredictAsync(It.IsAny<ChainValues>(), It.IsAny<CancellationToken>()))
            .Returns<ChainValues>(value => Task.FromResult((object)predict(value)));

        mock.Setup(x => x.Prompt)
            .Returns(promptTemplate);
        
        mock.Setup(x => x.InputKeys)
            .Returns(Array.Empty<string>());

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
}
using LangChain.Abstractions.Schema;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Sources;
using LangChain.Prompts;
using LangChain.Providers;
using LangChain.Schema;
using Moq;

namespace LangChain.Core.UnitTests.Chains.CombineDocuments;

[TestFixture]
public class MapReduceDocumentsChainTests
{
    [Test]
    public async Task MapReduce()
    {
        var theme1 = (Theme: "First", Mapped: "First theme text. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.");
        var theme2 = (Theme: "Second", Mapped: "Second theme text. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.");

        var document1 = new Document(theme1.Theme);
        var document2 = new Document(theme2.Theme);

        var llmChain = CreateFakeLlmChain(
            values =>
            {
                var valuesTheme = values.Value["theme"].ToString();
                if (valuesTheme == theme1.Theme) return theme1.Mapped;
                if (valuesTheme == theme2.Theme) return theme2.Mapped;
                throw new NotImplementedException($"unknown theme {valuesTheme}");
            },
            PromptTemplate.FromTemplate("Elaborate this theme: {theme}"),
            "theme",
            "result");

        var reduceDocumentsChain = new Mock<BaseCombineDocumentsChain>(new Mock<BaseCombineDocumentsChainInput>().Object);
        reduceDocumentsChain.Setup(x => x
                .CombineDocsAsync(
                    It.IsAny<IReadOnlyList<Document>>(),
                    It.IsAny<IReadOnlyDictionary<string, object>>(),
                    It.IsAny<CancellationToken>()))
            .Returns<IReadOnlyList<Document>, IReadOnlyDictionary<string, object>, CancellationToken>(
                (docs, otherKeys, _) =>
                    Task.FromResult(("mapreduced", new Dictionary<string, object>())));

        var input = new MapReduceDocumentsChainInput
        {
            LlmChain = llmChain.Object,
            ReduceDocumentsChain = reduceDocumentsChain.Object,
            ReturnIntermediateSteps = true,
            DocumentVariableName = "theme"
        };

        var chain = new MapReduceDocumentsChain(input);

        chain.InputKeys.Should().HaveCount(1);
        chain.InputKeys[0].Should().BeEquivalentTo("input_documents");

        chain.OutputKeys.Should().HaveCount(2);
        chain.OutputKeys[0].Should().BeEquivalentTo("output_text");
        chain.OutputKeys[1].Should().BeEquivalentTo("intermediate_steps");

        // chain.DocumentVariableName.Should().BeEquivalentTo("input");

        chain.ChainType().Should().BeEquivalentTo("map_reduce_documents_chain");

        var result = await chain.CombineDocsAsync(new [] { document1, document2 }, new Dictionary<string, object>());

        result.Output.Should().BeEquivalentTo("mapreduced");

        result.OtherKeys.Should().HaveCount(1);
        result.OtherKeys.Should().ContainKey("intermediate_steps");
        result.OtherKeys["intermediate_steps"].Should().BeOfType<List<object>>();
        var intermediateSteps = result.OtherKeys["intermediate_steps"] as List<object>;
        intermediateSteps.Should().HaveCount(2);
        intermediateSteps![0].Should().BeEquivalentTo(theme1.Mapped);
        intermediateSteps[1].Should().BeEquivalentTo(theme2.Mapped);

        reduceDocumentsChain
            .Verify(
                m => m.CombineDocsAsync(
                    It.Is<IReadOnlyList<Document>>(x =>
                        x.Count == 2 &&
                        x[0].PageContent == theme1.Mapped &&
                        x[1].PageContent == theme2.Mapped),
                    It.Is<IReadOnlyDictionary<string, object>>(x => !x.Any()),
                    It.IsAny<CancellationToken>()),
                Times.Once());
    }
    
    private Mock<ILlmChain> CreateFakeLlmChain(Func<ChainValues, string> predict, PromptTemplate promptTemplate, string inputKey, string outputKey)
    {
        var mock = new Mock<ILlmChain>();

        mock.Setup(x => x
                .ApplyAsync(It.IsAny<IReadOnlyList<ChainValues>>(), It.IsAny<CancellationToken>()))
            .Returns<IReadOnlyList<ChainValues>, CancellationToken>((values, _) =>
            {
                var result = values
                    .Select(value => new ChainValues(outputKey, predict(value)))
                    .Cast<IChainValues>()
                    .ToList();

                return Task.FromResult(result);
            });

        mock.Setup(x => x.Prompt)
            .Returns(promptTemplate);

        mock.Setup(x => x.InputKeys)
            .Returns(new[] { inputKey });

        mock.Setup(x => x.OutputKey)
            .Returns(outputKey);

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
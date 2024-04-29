using LangChain.Callback;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Sources;
using LangChain.Retrievers;
using Moq;

namespace LangChain.Core.UnitTests.Chains.RetrievalQa;

[TestFixture]
public class RetrievalQaChainTests
{
    [Test]
    public async Task Retrieval_Ok()
    {
        var retrieverMock = CreateRetrieverMock();
        var combineDocumentsMock = CreateCombineDocumentsChainMock();

        var input = new RetrievalQaChainInput(combineDocumentsMock.Object, retrieverMock.Object);
        var chain = new RetrievalQaChain(input);

        var result = await chain.RunAsync("question?");

        result.Should().BeEquivalentTo("answer");

        retrieverMock
            .Verify(
                m => m.GetRelevantDocumentsAsync(
                    It.Is<string>(x => x == "question?"),
                    It.IsAny<string>(),
                    It.IsAny<ICallbacks?>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());

        combineDocumentsMock
            .Verify(m => m.RunAsync(
                    It.Is<Dictionary<string, object>>(x =>
                        x["input_documents"].As<List<Document>>()
                            .Select(doc => doc.PageContent)
                            .Intersect(new string[] { "first", "second", "third" })
                            .Count() == 3),
                    It.IsAny<ICallbacks?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
    }

    private Mock<BaseRetriever> CreateRetrieverMock()
    {
        var mock = new Mock<BaseRetriever>();

        mock.Setup(x => x
                .GetRelevantDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ICallbacks>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<CancellationToken>()))
            .Returns<string, string, ICallbacks, bool, List<string>, Dictionary<string, object>>((query, _, _, _, _, _) =>
            {
                var docs = new List<Document>
                {
                    CreateDocument("first"),
                    CreateDocument("second"),
                    CreateDocument("third")
                };

                return Task.FromResult<IReadOnlyCollection<Document>>(docs);
            });

        return mock;
    }

    private Mock<BaseCombineDocumentsChain> CreateCombineDocumentsChainMock()
    {
        var mock = new Mock<BaseCombineDocumentsChain>(new Mock<BaseCombineDocumentsChainInput>().Object);

        mock.Setup(x => x
                .RunAsync(It.IsAny<Dictionary<string, object>>(), It.IsAny<ICallbacks?>(), It.IsAny<CancellationToken>()))
            .Returns<Dictionary<string, object>, ICallbacks>((input, _) => Task.FromResult("answer"));

        return mock;
    }

    private Document CreateDocument(string content) => new(content, new Dictionary<string, object>());
}
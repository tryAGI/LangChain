using LangChain.Callback;
using LangChain.Chains.CombineDocuments;
using LangChain.Chains.RetrievalQA;
using LangChain.Docstore;
using LangChain.Retrievers;
using Moq;

namespace LangChain.Core.UnitTests.Chains.RetrievalQa;

[TestClass]
public class RetrievalQaChainTests
{
    [TestMethod]
    public async Task Retrieval_Ok()
    {
        var retrieverMock = CreateRetrieverMock();
        var combineDocumentsMock = CreateCombineDocumentsChainMock();

        var input = new RetrievalQaChainInput(combineDocumentsMock.Object, retrieverMock.Object);
        var chain = new RetrievalQaChain(input);

        var result = await chain.Run("question?");

        result.Should().BeEquivalentTo("answer");

        retrieverMock
            .Verify(
                m => m.GetRelevantDocumentsAsync(
                    It.Is<string>(x => x == "question?"),
                    It.IsAny<string>(),
                    It.IsAny<Callbacks?>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once());

        combineDocumentsMock
            .Verify(m => m.Run(
                    It.Is<Dictionary<string, object>>(x =>
                        x["input_documents"].As<List<Document>>()
                            .Select(doc => doc.PageContent)
                            .Intersect(new string[] { "first", "second", "third" })
                            .Count() == 3)),
                Times.Once());
    }

    private Mock<BaseRetriever> CreateRetrieverMock()
    {
        var mock = new Mock<BaseRetriever>();
        
        mock.Setup(x => x
                .GetRelevantDocumentsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Callbacks>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<Dictionary<string, object>>()))
            .Returns<string, string, Callbacks, bool, List<string>, Dictionary<string, object>>((query, _, _, _, _, _) =>
            {
                var docs = new List<Document>
                {
                    CreateDocument("first"),
                    CreateDocument("second"),
                    CreateDocument("third")
                }.AsEnumerable();

                return Task.FromResult(docs);
            });

        return mock;
    }

    private Mock<BaseCombineDocumentsChain> CreateCombineDocumentsChainMock()
    {
        var mock = new Mock<BaseCombineDocumentsChain>(new Mock<BaseCombineDocumentsChainInput>().Object);

        mock.Setup(x => x
                .Run(It.IsAny<Dictionary<string, object>>()))
            .Returns<Dictionary<string, object>>(input => Task.FromResult("answer"));

        return mock;
    }

    private Document CreateDocument(string content) => new(content, new ());
}
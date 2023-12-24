using LangChain.Chains.CombineDocuments;
using LangChain.Chains.LLM;
using LangChain.Docstore;
using LangChain.Prompts;
using LangChain.Schema;
using Moq;

namespace LangChain.Core.UnitTests.Chains.CombineDocuments;

[TestFixture]
public class StuffDocumentsChainTests
{
    [Test]
    public async Task CombineDocsAsync_CombineDocs_Ok()
    {
        var llmChain = CreateFakeLlmChain();
        var input = new StuffDocumentsChainInput(llmChain.Object);
        var chain = new StuffDocumentsChain(input);

        chain.InputKeys.Should().HaveCount(1);
        chain.InputKeys[0].Should().BeEquivalentTo("input_documents");

        chain.OutputKeys.Should().HaveCount(1);
        chain.OutputKeys[0].Should().BeEquivalentTo("output_text");

        chain.ChainType().Should().BeEquivalentTo("stuff_documents_chain");

        var docs = new List<Document>
        {
            new Document("First page text"),
            new Document("Second page different text")
        };

        var result = await chain.CombineDocsAsync(docs, new Dictionary<string, object>());

        result.Output.Should().BeEquivalentTo("predict response");
        result.OtherKeys.Should().BeEmpty();

        llmChain
            .Verify(
                m => m.Predict(
                    It.Is<ChainValues>(x => x.Value["documents_content"].ToString() == "First page text\n\nSecond page different text")),
                Times.Once());
    }

    [Test]
    public async Task CombineDocsAsync_CustomPromptAndSeparator_Ok()
    {
        var documentVariableName = "different_name";
        var otherKey = "other_key";

        var templateInput = new PromptTemplateInput($"{{{otherKey}}}{{{documentVariableName}}}", new[] { documentVariableName, otherKey });
        var llmChain = CreateFakeLlmChain(templateInput);

        var input = new StuffDocumentsChainInput(llmChain.Object)
        {
            DocumentVariableName = documentVariableName,
            DocumentPrompt = new PromptTemplate(
                new PromptTemplateInput(
                "Page={page}.Content={page_content}",
                new[] { "page", "page_content" })),
            DocumentSeparator = "+"
        };

        var chain = new StuffDocumentsChain(input);

        var docs = new List<Document>
        {
            CreateDocument("First", ("page", 1)),
            CreateDocument("Second", ("page", 2))
        };

        var result = await chain.CombineDocsAsync(docs, new Dictionary<string, object> { [otherKey] = "hello!" });

        result.Output.Should().BeEquivalentTo("predict response");
        result.OtherKeys.Should().BeEmpty();

        llmChain
            .Verify(
                m => m.Predict(
                    It.Is<ChainValues>(x =>
                        x.Value[documentVariableName].ToString() == "Page=1.Content=First+Page=2.Content=Second" &&
                        x.Value[otherKey].ToString() == "hello!")),
                Times.Once());
    }

    private Mock<ILlmChain> CreateFakeLlmChain(PromptTemplateInput? documentsVariableName = null)
    {
        var mock = new Mock<ILlmChain>();

        mock.Setup(x => x
                .Predict(It.IsAny<ChainValues>()))
            .Returns<ChainValues>(_ => Task.FromResult((object)"predict response"));
        
        mock.Setup(x => x.InputKeys)
            .Returns(Array.Empty<string>());

        var templateInput = documentsVariableName ?? new PromptTemplateInput("{documents_content}", new[] { "documents_content" });
        var prompt = new PromptTemplate(templateInput);

        mock.Setup(x => x.Prompt).Returns(prompt);

        return mock;
    }

    private Document CreateDocument(string content, params (string Key, object Value)[] metadatas)
    {
        var metadata = metadatas.ToDictionary(kv => kv.Key, kv => kv.Value);

        return new Document(content, metadata);
    }
}
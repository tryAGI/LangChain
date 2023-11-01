namespace LangChain.Sources.Pdf.IntegrationTests;

[TestClass]
public class PdfSourceTests
{
    [TestMethod]
    public async Task PdfPig_CheckText()
    {
        var loader = new PdfPigPdfSource
        {
            Path = "sample.pdf"
        };

        var documents = await loader.LoadAsync();

        documents.Should().NotBeEmpty();
        var first = documents.First();

        // check text from page 1
        first.Content.Should().Contain("A Simple PDF File");

        // check text from page 2
        first.Content.Should().Contain("Simple PDF File 2");
    }
}
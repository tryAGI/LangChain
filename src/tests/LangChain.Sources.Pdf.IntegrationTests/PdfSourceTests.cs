namespace LangChain.Sources.Pdf.IntegrationTests;

[TestClass]
public class PdfSourceTests
{
    [TestMethod]
    public async Task PdfPig_CheckText()
    {
        var loader = new PdfPigPdfSource("sample.pdf");

        var documents = await loader.LoadAsync();

        documents.Should().NotBeEmpty();


        // check text from page 1
        documents.First().PageContent.Should().Contain("A Simple PDF File");

        // check text from page 2
        documents.Skip(1).First().PageContent.Should().Contain("Simple PDF File 2");
    }
}
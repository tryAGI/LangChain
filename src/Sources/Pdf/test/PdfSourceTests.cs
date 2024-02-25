namespace LangChain.Sources.Pdf.IntegrationTests;

[TestFixture]
public class PdfSourceTests
{
    // CICD exception UglyToad.PdfPig.Core.PdfDocumentFormatException : Expected name as dictionary key, instead got: 0
    // [Test]
    // public async Task PdfPig_CheckText()
    // {
    //     var loader = new PdfPigPdfSource(H.Resources.sample_pdf.AsStream());
    //
    //     var documents = await loader.LoadAsync();
    //
    //     documents.Should().NotBeEmpty();
    //
    //
    //     // check text from page 1
    //     documents.First().PageContent.Should().Contain("A Simple PDF File");
    //
    //     // check text from page 2
    //     documents.Skip(1).First().PageContent.Should().Contain("Simple PDF File 2");
    // }
}
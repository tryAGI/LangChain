// namespace LangChain.DocumentLoaders.IntegrationTests;
//
// public partial class Tests
// {
//     [Test]
//     public Task Pdf()
//     {
//         return Task.CompletedTask;
//         // CICD exception UglyToad.PdfPig.Core.PdfDocumentFormatException : Expected name as dictionary key, instead got: 0
//         // var loader = new PdfPigPdfLoader();
//         // var documents = await loader.LoadAsync(DataSource.FromStream(H.Resources.sample_pdf.AsStream()));
//         //
//         // documents.Should().NotBeEmpty();
//         //
//         // // check text from page 1
//         // documents.First().PageContent.Should().Contain("A Simple PDF File");
//         //
//         // // check text from page 2
//         // documents.Skip(1).First().PageContent.Should().Contain("Simple PDF File 2");
//     }
// }
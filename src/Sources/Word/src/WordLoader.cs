using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed class WordLoader : IDocumentLoader
{
    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Document>> LoadAsync(DataSource dataSource, CancellationToken cancellationToken = default)
    {
        dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

        using var stream = await dataSource.GetStreamAsync(cancellationToken).ConfigureAwait(false);
        using var wordDocument = WordprocessingDocument.Open(stream, isEditable: false);

        var documents = new List<string>();
        foreach (var paragraph in wordDocument.MainDocumentPart?.Document.Body?.Elements<Paragraph>() ?? [])
        {
            var paragraphText = new StringBuilder();
            foreach (var run in paragraph.Elements<Run>())
            {
                paragraphText.Append(run.InnerText);
            }

            if (paragraphText.Length > 0)
            {
                documents.Add(paragraphText.ToString());
            }
        }

        return documents
            .Select(text => new Document(text, metadata: new Dictionary<string, object>
            {
                ["path"] = dataSource.Value ?? string.Empty,
                ["type"] = dataSource.Type.ToString(),
            }))
            .ToList();
    }
}
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public sealed partial class WordSource : ISource
#if NET7_0_OR_GREATER
    , ICreatableFromStream<WordSource>
#endif
{
    public static WordSource CreateFromStream(Stream stream)
    {
        return new WordSource(stream);
    }

    public string FilePath { get; init; } = string.Empty;

    public Stream? Stream { get; }

    public WordSource(string path)
    {
        FilePath = path;
    }

    public WordSource(Stream stream)
    {
        Stream = stream;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<Document>> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = new List<string>();

            using var wordDocument = Stream != null
                ? WordprocessingDocument.Open(Stream, isEditable: false)
                : WordprocessingDocument.Open(FilePath, isEditable: false);

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

            return Task.FromResult<IReadOnlyCollection<Document>>(documents
                .Select(text => new Document(text, metadata: new Dictionary<string, object>
                {
                    ["path"] = FilePath,
                }))
                .ToList());
        }
        catch (Exception exception)
        {
            return Task.FromException<IReadOnlyCollection<Document>>(exception);
        }
    }
}
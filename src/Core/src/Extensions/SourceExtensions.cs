using LangChain.Sources;
using LangChain.Splitters.Text;

namespace LangChain.Extensions;

public static class SourceExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="documentLoader"></param>
    /// <param name="dataSource"></param>
    /// <param name="textSplitter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyList<Document>> LoadAndSplit(
        this IDocumentLoader documentLoader,
        DataSource dataSource,
        ITextSplitter? textSplitter = null,
        CancellationToken cancellationToken = default)
    {
        documentLoader = documentLoader ?? throw new ArgumentNullException(nameof(documentLoader));
        dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        textSplitter ??= new RecursiveCharacterTextSplitter();

        var documents = await documentLoader.LoadAsync(dataSource, cancellationToken).ConfigureAwait(false);
        
        return textSplitter.SplitDocuments(documents);
    }
}
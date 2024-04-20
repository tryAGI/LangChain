using LangChain.Sources;
using LangChain.Splitters.Text;

namespace LangChain.Extensions;

public static class SourceExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="textSplitter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IReadOnlyList<Document>> LoadAndSplit(
        this ISource source,
        ITextSplitter? textSplitter = null,
        CancellationToken cancellationToken = default)
    {
        source = source ?? throw new ArgumentNullException(nameof(source));
        textSplitter ??= new RecursiveCharacterTextSplitter();
        
        var documents = await source.LoadAsync(cancellationToken).ConfigureAwait(false);
        return textSplitter.SplitDocuments(documents);
    }
}
namespace LangChain.Splitters.Code;

/// <summary>
/// 
/// </summary>
public interface ICodeSplitter
{
    /// <summary>
    /// Divides a document into its component parts, returning the title, type, and content of each part.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<CodePart>> SplitAsync(
        string content,
        CancellationToken cancellationToken = default);
}
namespace LangChain.Splitters;

/// <summary>
/// 
/// </summary>
public interface ISplitter
{
    /// <summary>
    /// Divides a document into its component parts, returning the title, type, and content of each part.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyCollection<DocumentPart>> SplitAsync(
        string content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Trims the selected text so that it includes only the required things and the specified names after splitting.
    /// </summary>
    /// <param name="requiredNames"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public Task<string> CutAsync(
        string content,
        IReadOnlyCollection<string> requiredNames,
        CancellationToken cancellationToken = default);
}
namespace LangChain.Splitters.Code;

/// <summary>
/// 
/// </summary>
public interface ICodeCutter
{
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
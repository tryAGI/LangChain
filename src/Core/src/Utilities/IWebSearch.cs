namespace LangChain.Utilities;

/// <summary>
/// 
/// </summary>
public interface IWebSearch
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<string> RunAsync(string query);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="numResults"></param>
    /// <returns></returns>
    Task<List<WebSearchResult>> ResultsAsync(string query, int numResults);
}

/// <summary>
/// 
/// </summary>
/// <param name="Title"></param>
/// <param name="Body"></param>
/// <param name="Link"></param>
public readonly record struct WebSearchResult(
    string Title,
    string Body,
    string Link);
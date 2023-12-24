namespace LangChain.Utilities;

/// <summary>
/// Wrapper for DuckDuckGo Search API.
/// 
/// Free and does not require any setup.
/// </summary>
public sealed class DuckDuckGoSearchApiWrapper(
    string region = "wt-wt",
    DuckDuckGoSearch.SafeSearchType safeSearch = DuckDuckGoSearch.SafeSearchType.Moderate,
    DuckDuckGoSearch.TimeLimit time = DuckDuckGoSearch.TimeLimit.Year,
    int maxResults = 5)
    : IWebSearch, IDisposable
{
    private readonly DuckDuckGoSearch _search = new();

    /// <summary>
    /// Get aggregated search result
    /// </summary>
    public async Task<string> RunAsync(string query)
    {
        var snippets = await GetSnippetsAsync(query).ConfigureAwait(false);

        return String.Join(" ", snippets);
    }

    /// <summary>
    /// Run query through DuckDuckGo and return concatenated results.
    /// </summary>
    public async Task<IEnumerable<string>> GetSnippetsAsync(string query)
    {
        var results = _search.TextSearchAsync(
            query,
            region: region,
            safeSearch: safeSearch,
            timeLimit: time);

        var snippets = new List<string>();
        await foreach (var result in results)
        {
            snippets.Add(result["body"]);

            if (snippets.Count == maxResults)
            {
                break;
            }
        }

        if (snippets.Count == 0)
        {
            snippets.Add("No good DuckDuckGo Search Result was found");
        }

        return snippets;
    }

    /// <summary>
    /// Run query through DuckDuckGo and return metadata.
    /// </summary>
    /// <remarks>
    /// Only "api" backend supported
    /// </remarks>
    /// <param name="query">The query to search for.</param>
    /// <param name="numResults">The number of results to return.</param>
    /// <returns>
    /// A list of items with the following props:
    ///     title - The description of the result.
    ///     snippet - The title of the result.
    ///     link - The link to the result.
    /// </returns>
    public async Task<List<WebSearchResult>> ResultsAsync(
        string query,
        int numResults)
    {
        var results = _search.TextSearchAsync(
            query,
            region: region,
            safeSearch: safeSearch,
            timeLimit: time,
            maxResults: maxResults);

        var formattedResults = new List<WebSearchResult>();
        await foreach (var result in results)
        {
            var formattedResult = new WebSearchResult(
                Title: result["title"],
                Body: result["body"],
                Link: result["href"]);
            formattedResults.Add(formattedResult);

            if (formattedResults.Count == numResults)
            {
                break;
            }
        }

        return formattedResults;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _search.Dispose();
    }
}
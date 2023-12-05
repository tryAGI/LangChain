namespace LangChain.Utilities;

public interface IWebSearch
{
    Task<string> RunAsync(string query);
    Task<List<WebSearchResult>> ResultsAsync(string query, int numResults);
}

public class WebSearchResult(string title, string body, string link)
{
    public string Title { get; set; } = title;
    public string Body { get; set; } = body;
    public string Link { get; set; } = link;
}
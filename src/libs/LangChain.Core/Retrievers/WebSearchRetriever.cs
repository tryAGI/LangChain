using LangChain.Callback;
using LangChain.Docstore;
using LangChain.Utilities;

namespace LangChain.Retrievers;

public sealed class WebSearchRetriever : BaseRetriever
{
    private readonly IWebSearch _webSearch;
    private readonly int _k;

    public WebSearchRetriever(IWebSearch webSearch, int k = 10)
    {
        _webSearch = webSearch;
        _k = k;
    }

    protected override async Task<IEnumerable<Document>> GetRelevantDocumentsCoreAsync(
        string query,
        CallbackManagerForRetrieverRun runManager = null)
    {
        var searchResult = await _webSearch.ResultsAsync(query, _k);

        return searchResult.Select(v => new Document(
            v.Body,
            new Dictionary<string, object>()
            {
                ["title"] = v.Title,
                ["link"] = v.Link
            }));
    }
}
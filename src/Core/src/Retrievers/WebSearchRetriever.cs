using LangChain.Callback;
using LangChain.DocumentLoaders;
using LangChain.Utilities;

namespace LangChain.Retrievers;

/// <inheritdoc/>
public sealed class WebSearchRetriever(
    IWebSearch webSearch,
    int k = 10)
    : BaseRetriever
{
    /// <inheritdoc/>
    protected override async Task<IEnumerable<Document>> GetRelevantDocumentsCoreAsync(
        string query,
        CallbackManagerForRetrieverRun? runManager = null,
        CancellationToken cancellationToken = default)
    {
        var searchResult = await webSearch.ResultsAsync(query, k, cancellationToken).ConfigureAwait(false);

        return searchResult.Select(v => new Document(
            v.Body,
            new Dictionary<string, object>
            {
                ["title"] = v.Title,
                ["link"] = v.Link
            }));
    }
}
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LangChain.Utilities;

/// <summary>
/// DuckDuckGo search client
/// </summary>
public sealed class DuckDuckGoSearch : IDisposable
{
    private readonly HttpClient _client = new(
        new HttpClientHandler
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 2
        });

    private readonly Regex _stringTagsRegex = new Regex("<.*?>", RegexOptions.Compiled);
    private readonly Regex _regex500InUrl = new Regex("(?:\\d{3}-\\d{2}\\.js)", RegexOptions.Compiled);

    /// <summary>
    /// DuckDuckGo text search generator. Query params: https://duckduckgo.com/params
    ///
    /// <see cref="https://github.com/deedy5/duckduckgo_search/blob/main/duckduckgo_search/duckduckgo_search_async.py"/>
    /// </summary>
    /// <param name="keywords">keywords for query</param>
    /// <param name="region"><see cref="https://serpapi.com/duckduckgo-regions"/></param>
    /// <param name="safeSearch"><see cref="SafeSearchType"/></param>
    /// <param name="timeLimit"><see cref="TimeLimit"/></param>
    /// <param name="backend">
    /// api, html, lite. Defaults to api.
    ///     api - collect data from https://duckduckgo.com,
    ///     html - collect data from https://html.duckduckgo.com,
    ///     lite - collect data from https://lite.duckduckgo.com.
    /// </param>
    /// <param name="maxResults">max number of results. If null, returns results only from the first response</param>
    /// <returns></returns>
    public async IAsyncEnumerable<Dictionary<string, string>> TextSearchAsync(
        string keywords,
        string region = "wt-wt",
        SafeSearchType safeSearch = SafeSearchType.Moderate,
        TimeLimit? timeLimit = null,
        int? maxResults = null)
    {
        var results = TextSearchApiAsync(keywords, region, safeSearch, timeLimit, maxResults);
        var resultsCounter = 0;
        await foreach (var result in results)
        {
            yield return result;
            resultsCounter += 1;
            if (maxResults != null && resultsCounter >= maxResults)
            {
                yield break;
            }
        }
    }

    private async IAsyncEnumerable<Dictionary<string, string>> TextSearchApiAsync(
        string keywords,
        string region,
        SafeSearchType safeSearch,
        TimeLimit? timeLimit,
        int? maxResults)
    {
        var payload = await GetPayloadAsync(keywords, region, safeSearch, timeLimit);

        var i = 0;
        var cache = new HashSet<string>();
        while (i++ <= 10)
        {
            var response = await HttpGetAsync("https://links.duckduckgo.com/d.js", payload);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                yield break;
            }

            LinksResponse.LinksResponseItem[]? pageData;
            try
            {
                var contentRaw = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<LinksResponse>(contentRaw);

                pageData = content?.Results;
                if (pageData == null || pageData.Length == 0)
                {
                    yield break;
                }
            }
            catch
            {
                yield break;
            }

            string? nextPageUrl = null;
            var resultExists = false;
            foreach (var row in pageData)
            {
                var href = row.Url;
                if (href != null &&
                    !cache.Contains(href) &&
                    href != $"http://www.google.com/search?q={keywords}")
                {
                    cache.Add(href);
                    var body = NormalizeHtml(row.Body);
                    if (!String.IsNullOrEmpty(body))
                    {
                        resultExists = true;
                        yield return new Dictionary<string, string>
                        {
                            ["title"] = NormalizeHtml(row.Title),
                            ["href"] = NormalizeUrl(href),
                            ["body"] = body,
                        };
                    }
                }
                else
                {
                    nextPageUrl = row.NextPageUrl;
                }
            }

            if (maxResults == null || resultExists == false || String.IsNullOrEmpty(nextPageUrl))
            {
                yield break;
            }

            var separator = new[] { "s=" };
            payload["s"] = nextPageUrl.Split(separator, StringSplitOptions.RemoveEmptyEntries)[1].Split('&')[0];

            await Sleep();
        }
    }

    private async Task<Dictionary<string, string>> GetPayloadAsync(
        string keywords,
        string region,
        SafeSearchType safeSearch, TimeLimit? timeLimit)
    {
        var vqd = await GetVqdAsync(keywords);

        var timeLimitString = timeLimit switch
        {
            TimeLimit.Day => "d",
            TimeLimit.Week => "w",
            TimeLimit.Month => "m",
            TimeLimit.Year => "y",
            _ => String.Empty
        };

        var payload = new Dictionary<string, string>
        {
            ["q"] = keywords,
            ["kl"] = region,
            ["l"] = region,
            ["bing_market"] = region,
            ["s"] = "0",
            ["df"] = timeLimitString,
            ["vqd"] = vqd,
            ["o"] = "json",
            ["sp"] = "0",
        };

        switch (safeSearch)
        {
            case SafeSearchType.Moderate:
                payload["ex"] = "-1";
                break;
            case SafeSearchType.Off:
                payload["ex"] = "-2";
                break;
            case SafeSearchType.On:
                payload["p"] = "1";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(safeSearch), safeSearch, null);
        }

        return payload;
    }

    /// <summary>
    /// Unquote URL and replace spaces with '+'
    /// </summary>
    private static string NormalizeUrl(string url)
    {
        if (String.IsNullOrEmpty(url))
        {
            return String.Empty;
        }

        return WebUtility.UrlDecode(url.Replace(" ", "+"));
    }

    /// <summary>
    /// Strip HTML tags from the raw_html string.
    /// </summary>
    private string NormalizeHtml(string rawHtml)
    {
        if (String.IsNullOrEmpty(rawHtml))
        {
            return String.Empty;
        }

        var html = _stringTagsRegex.Replace(rawHtml, "");

        return WebUtility.HtmlDecode(html);
    }

    private class LinksResponse
    {
        [JsonInclude]
        [JsonPropertyName("results")]
        public LinksResponseItem[]? Results { get; private set; }

        public class LinksResponseItem
        {
            [JsonInclude]
            [JsonPropertyName("u")]
            public string? Url { get; private set; }

            [JsonInclude]
            [JsonPropertyName("t")]
            public string? Title { get; private set; }

            [JsonInclude]
            [JsonPropertyName("a")]
            public string? Body { get; private set; }

            [JsonInclude]
            [JsonPropertyName("n")]
            public string? NextPageUrl { get; private set; }
        }
    }

    /// <summary>
    /// Sleep between API requests if proxies is None.
    /// </summary>
    private async Task Sleep()
    {
        // TODO: if (proxies == null)
        await Task.Delay(750);
    }

    /// <summary>
    /// Get vqd value for a search query.
    /// </summary>
    /// <param name="keywords"></param>
    /// <returns></returns>
    private async Task<string> GetVqdAsync(string keywords)
    {
        var resp = await HttpGetAsync(
            "https://duckduckgo.com",
            new Dictionary<string, string>
            {
                ["q"] = keywords
            });

        if (resp.StatusCode == HttpStatusCode.OK)
        {
            var content = await resp.Content.ReadAsStringAsync();

            var vqdIndex = content.IndexOf("vqd=", StringComparison.Ordinal);
            if (vqdIndex > 0)
            {
                var start = vqdIndex + "vqd=".Length;
                var nextChar = content[start];

                char endToken;
                if (nextChar == '\'')
                {
                    start += 1;
                    endToken = '\'';
                }
                else if (nextChar == '\"')
                {
                    start += 1;
                    endToken = '\"';
                }
                else
                {
                    endToken = '&';
                }

                var end = content.IndexOf(endToken, start);

                return content.Substring(start, end - start);
            }
        }

        throw new VqdExtractionException($"Could not extract vqd. {keywords}");
    }

    private static string AddQueryParamsToUrl(string baseUrl, Dictionary<string, string> queryParameters)
    {
        var queryParts = new List<string>();
        foreach (var queryParameter in queryParameters)
        {
            var encodedKey = WebUtility.UrlEncode(queryParameter.Key);
            var encodedValue = WebUtility.UrlEncode(queryParameter.Value);

            queryParts.Add($"{encodedKey}={encodedValue}");
        }

        var url = $"{baseUrl}?{String.Join("&", queryParts)}";

        return url;
    }

    private async Task<HttpResponseMessage> HttpGetAsync(string url, Dictionary<string, string> queryParams)
    {
        var urlWithQuery = AddQueryParamsToUrl(url, queryParams);

        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await _client.GetAsync(urlWithQuery);
        }
        catch (TaskCanceledException e)
        {
            throw new TimeoutException($"HttpGetAsync {urlWithQuery}", e);
        }
        catch (Exception e)
        {
            throw new DuckDuckGoSearchException($"HttpGetAsync {urlWithQuery}. {e.GetType()}: {e}", e);
        }

        var lastUrl = responseMessage.RequestMessage?.RequestUri?.ToString();
        if (lastUrl != null && Is500InUrl(lastUrl))
        {
            throw new ApiException($"HttpGetAsync {urlWithQuery}");
        }

        if (responseMessage.StatusCode == HttpStatusCode.Accepted)
        {
            throw new RateLimitException($"HttpGetAsync {urlWithQuery}");
        }

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            return responseMessage;
        }

        throw new HttpRequestException($"HttpGetAsync finished with status code: {responseMessage.StatusCode}");
    }

    /// <summary>
    /// something like '506-00.js' inside the url
    /// </summary>
    private bool Is500InUrl(string url)
    {
        return _regex500InUrl.IsMatch(url);
    }

    /// <inheritdoc />
    public class VqdExtractionException(string? message)
        : Exception(message);

    /// <inheritdoc />
    public class DuckDuckGoSearchException(string? message, Exception innerException)
        : Exception(message, innerException);

    /// <inheritdoc />
    public class ApiException(string? message)
        : Exception(message);

    /// <inheritdoc />
    public class TimeoutException(string? message, Exception innerException)
        : Exception(message, innerException);

    /// <inheritdoc />
    public class RateLimitException(string? message)
        : Exception(message);

    public enum TimeLimit
    {
        Day,
        Week,
        Month,
        Year,
    }

    public enum SafeSearchType
    {
        On,
        Moderate,
        Off
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
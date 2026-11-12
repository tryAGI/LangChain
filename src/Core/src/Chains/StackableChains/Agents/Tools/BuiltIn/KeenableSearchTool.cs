using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn.Classes;

namespace LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;

/// <summary>
/// Web search with the Keenable Search API. Works without an API key. <br/>
/// Without a key, requests go to the public endpoint (1,000 requests per hour per IP, see https://docs.keenable.ai/rate-limits). <br/>
/// Pass an API key, or set the KEENABLE_API_KEY environment variable, to remove the hourly cap.
/// Keys are created at https://keenable.ai.
/// </summary>
/// <param name="apiKey">Optional Keenable API key. Falls back to the KEENABLE_API_KEY environment variable.</param>
/// <param name="useCache"></param>
/// <param name="resultsLimit"></param>
/// <param name="appTitle">Sent as the X-Keenable-Title header on keyless requests.</param>
/// <param name="httpClient">Optional HttpClient, for example for tests or a proxy.</param>
public class KeenableSearchTool(
    string? apiKey = null,
    bool useCache = true,
    int resultsLimit = 3,
    string appTitle = "LangChain .NET",
    HttpClient? httpClient = null)
    : AgentTool(
        name: "keenable",
        description: "to search information on internet")
{
    private const string CacheDir = "cache";

    private static string Hash(string prompt)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(prompt));

        return hash.Aggregate("", (current, b) => current + $"{b:x2}");
    }

    private static string? GetCachedAnswer(string prompt)
    {
        var file = Path.Combine(CacheDir, $"{Hash(prompt)}.keenablecache");
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }

        return null;
    }

    private static void SaveCachedAnswer(string prompt, string answer)
    {
        Directory.CreateDirectory(CacheDir);
        var file = Path.Combine(CacheDir, $"{Hash(prompt)}.keenablecache");
        File.WriteAllText(file, answer);
    }

    public override async Task<string> ToolTask(string input, CancellationToken cancellationToken = default)
    {
        string? responseString;
        if (!useCache || (responseString = GetCachedAnswer(input)) == null)
        {
            var key = apiKey ?? Environment.GetEnvironmentVariable("KEENABLE_API_KEY");
            var hasKey = !string.IsNullOrWhiteSpace(key);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                hasKey
                    ? "https://api.keenable.ai/v1/search"
                    : "https://api.keenable.ai/v1/search/public");
            if (hasKey)
            {
                request.Headers.Add("X-API-Key", key);
            }
            else
            {
                request.Headers.Add("X-Keenable-Title", appTitle);
            }

            var body = JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["query"] = input,
                ["max_results"] = resultsLimit,
            });
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpClient? ownedClient = httpClient == null ? new HttpClient() : null;
            var client = httpClient ?? ownedClient!;
            try
            {
                using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ownedClient?.Dispose();
            }
        }

        var results = JsonSerializer.Deserialize<KeenableResult>(responseString!)!;

        var stringBuilder = new StringBuilder();
        int count = 0;
        foreach (var result in results.Results)
        {
            stringBuilder.AppendLine($"{result.Title}");
            stringBuilder.AppendLine($"{result.Snippet}");
            stringBuilder.AppendLine($"Source url: {result.Link}");
            stringBuilder.AppendLine();
            if (++count >= resultsLimit)
                break;
        }

        if (results.Results.Count == 0)
        {
            stringBuilder.AppendLine("No results found");
            stringBuilder.AppendLine();
        }

        if (useCache)
            SaveCachedAnswer(input, responseString);

        return stringBuilder.ToString();
    }
}

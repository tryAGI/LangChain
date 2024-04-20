using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn.Classes;

namespace LangChain.Chains.StackableChains.Agents.Tools.BuiltIn;

/// <summary>
/// To use it you need to get key and cx from Google. <br/>
/// Custom Search JSON API provides 100 search queries per day for free. If you need more, you may sign up for billing in the API Console. Additional requests cost $5 per 1000 queries, up to 10k queries per day. <br/>
/// To get api key go here: https://developers.google.com/custom-search/v1/overview. <br/>
/// You need to create Programmable Search Engine to get cx(It's Code in Programmable Search Engine Overview)
/// </summary>
/// <param name="key"></param>
/// <param name="cx"></param>
/// <param name="useCache"></param>
/// <param name="resultsLimit"></param>
public class GoogleCustomSearchTool(
    string key,
    string cx,
    bool useCache = true,
    int resultsLimit = 3)
    : AgentTool(
        name: "google",
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
        var file = Path.Combine(CacheDir, $"{Hash(prompt)}.googlecache");
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }

        return null;
    }

    private static void SaveCachedAnswer(string prompt, string answer)
    {
        Directory.CreateDirectory(CacheDir);
        var file = Path.Combine(CacheDir, $"{Hash(prompt)}.googlecache");
        File.WriteAllText(file, answer);
    }

    public override async Task<string> ToolTask(string input, CancellationToken cancellationToken = default)
    {
        string? responseString;
        if (!useCache||(responseString = GetCachedAnswer(input))==null)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.googleapis.com");
            var response = await httpClient.GetAsync(
                new Uri($"/customsearch/v1?key={key}&cx={cx}&q={input}", UriKind.Relative), cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        
        var results = JsonSerializer.Deserialize<GoogleResult>(responseString!)!;

        var stringBuilder = new StringBuilder();
        int count = 0;
        foreach (var result in results.items)
        {
            stringBuilder.AppendLine($"{result.title}");
            stringBuilder.AppendLine($"{result.snippet}");
            stringBuilder.AppendLine($"Source url: {result.link}");
            stringBuilder.AppendLine();
            if (++count >= resultsLimit)
                break;
        }

        if (results.items.Count == 0)
        {
            stringBuilder.AppendLine("No results found");
            stringBuilder.AppendLine();
        }

        if (useCache)
            SaveCachedAnswer(input, responseString);
        
        return stringBuilder.ToString();
    }
}
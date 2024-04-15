using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LangChain.Chains.StackableChains.Agents.Crew.Tools;
using LangChain.Chains.StackableChains.Agents.Tools.BuiltIn.Classes;

namespace LangChain.IntegrationTests;

public class GoogleSearchTool(string key, string cx, bool useCache = true, int resultsLimit = 3)
    : CrewAgentTool("google", "to search information on internet")
{
    private const string CACHE_DIR = "cache";

    private string Hash(string prompt)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(prompt));
        string resultHex = "";
        foreach (var b in hash)
            resultHex += $"{b:x2}";
        return resultHex;
    }

    string? GetCachedAnswer(string prompt)
    {
        var file = Path.Combine(CACHE_DIR, $"{Hash(prompt)}.googlecache");
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }

        return null;
    }

    void SaveCachedAnswer(string prompt, string answer)
    {
        Directory.CreateDirectory(CACHE_DIR);
        var file = Path.Combine(CACHE_DIR, $"{Hash(prompt)}.googlecache");
        File.WriteAllText(file, answer);
    }

    public override async Task<string> ToolTask(string input, CancellationToken cancellationToken = default)
    {
        string? responseString;
        if (!useCache || (responseString = GetCachedAnswer(input)) == null)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.googleapis.com");
            var response = await httpClient.GetAsync(
                new Uri($"/customsearch/v1?key={key}&cx={cx}&q={input}"), cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        GoogleResult results = JsonSerializer.Deserialize<GoogleResult>(responseString!)!;

        var stringBuilder = new StringBuilder();
        int cnt = 0;
        foreach (var result in results.items)
        {
            stringBuilder.AppendLine($"Title: {result.title}");
            stringBuilder.AppendLine($"Link: {result.link}");
            stringBuilder.AppendLine($"Snippet: {result.snippet}");
            stringBuilder.AppendLine();
            if (++cnt >= resultsLimit)
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
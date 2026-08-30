using System.Security.Cryptography;
using System.Text;
using LangChain.Abstractions.Schema;
using Microsoft.Extensions.AI;

namespace LangChain.Chains.HelperChains;

/// <inheritdoc/>
public class LLMChain : BaseStackableChain
{
    private readonly IChatClient _chatClient;
    private bool _useCache;
    private ChatOptions _options;

    private const string CACHE_DIR = "cache";

    /// <inheritdoc/>
    public LLMChain(
        IChatClient chatClient,
        string inputKey = "prompt",
        string outputKey = "text",
        ChatOptions? options = null
        )
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
        _chatClient = chatClient;
        _options = options ?? new ChatOptions();
    }

    string? GetCachedAnswer(string prompt)
    {
        var modelId = _chatClient.GetService<ChatClientMetadata>()?.DefaultModelId ?? "unknown";
        var file = Path.Combine(CACHE_DIR, $"{modelId}.{Hash(prompt)}.llmcache");
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }
        return null;
    }

    void SaveCachedAnswer(string prompt, string answer)
    {
        Directory.CreateDirectory(CACHE_DIR);
        var modelId = _chatClient.GetService<ChatClientMetadata>()?.DefaultModelId ?? "unknown";
        var file = Path.Combine(CACHE_DIR, $"{modelId}.{Hash(prompt)}.llmcache");
        File.WriteAllText(file, answer);
    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var prompt = values.Value[InputKeys[0]].ToString() ?? string.Empty;
        string responseContent;

        if (_useCache)
        {
            var cached = GetCachedAnswer(prompt);
            if (cached != null)
            {
                responseContent = cached;
                values.Value[OutputKeys[0]] = responseContent;
                return values;
            }
        }

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, prompt),
        };
        var response = await _chatClient.GetResponseAsync(messages, _options, cancellationToken).ConfigureAwait(false);
        responseContent = response.Text ?? string.Empty;
        if (_useCache)
            SaveCachedAnswer(prompt, responseContent);
        values.Value[OutputKeys[0]] = responseContent;
        return values;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public LLMChain UseCache(bool enabled = true)
    {
        _useCache = enabled;
        return this;
    }

    private string Hash(string prompt)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(prompt));
        string resultHex = "";
        foreach (var b in hash)
            resultHex += $"{b:x2}";
        return resultHex;

    }
}

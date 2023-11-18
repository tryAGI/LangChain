using System.Security.Cryptography;
using System.Text;
using LangChain.Abstractions.Schema;
using LangChain.Callback;
using LangChain.Providers;

namespace LangChain.Chains.HelperChains;

public class LLMChain : BaseStackableChain
{
    private readonly IChatModel _llm;
    private bool _useCache;

    private const string CACHE_DIR = "cache";

    public LLMChain(IChatModel llm,
        string inputKey = "prompt",
        string outputKey = "text"
        )
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
        _llm = llm;
    }

    string? GetCachedAnswer(string prompt)
    {
        var file= Path.Combine(CACHE_DIR, $"{_llm.Id}.{Hash(prompt)}.llmcache");
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }
        return null;
    }

    void SaveCachedAnswer(string prompt, string answer)
    {
        Directory.CreateDirectory(CACHE_DIR);
        var file= Path.Combine(CACHE_DIR, $"{_llm.Id}.{Hash(prompt)}.llmcache");
        File.WriteAllText(file, answer);
    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        var prompt = values.Value[InputKeys[0]].ToString();
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
        
        
        var response = await _llm.GenerateAsync(new ChatRequest(new List<Message>() { prompt.AsSystemMessage() }));
        responseContent = response.Messages.Last().Content;
        if (_useCache)
            SaveCachedAnswer(prompt, responseContent);
        values.Value[OutputKeys[0]] = responseContent;
        return values;
    }

    public LLMChain UseCache(bool enabled=true)
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
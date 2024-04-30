﻿using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Providers;
using System.Security.Cryptography;
using System.Text;

namespace LangChain.Chains.StackableChains;

/// <summary>
/// Text to speech chain
/// </summary>
public class TTSChain : BaseStackableChain
{
    private readonly ITextToSpeechModel _model;
    private readonly TextToSpeechSettings? _settings;
    private readonly string _inputKey;
    private readonly string _outputKey;
    private bool _useCache;

    private const string CACHE_DIR = "cache";

    /// <inheritdoc/>
    public TTSChain(
        ITextToSpeechModel model,
        TextToSpeechSettings? settings = null,
        string inputKey = "text",
        string outputKey = "audio")
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
        _model = model;
        _settings = settings;
        _inputKey = inputKey;
        _outputKey = outputKey;

    }

    /// <inheritdoc/>
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var text = (string)values.Value[_inputKey];

        if (_useCache)
        {
            var cached = GetCachedAnswer(text);
            if (cached != null)
            {
                values.Value[_outputKey] = cached;
                return values;
            }
        }

        var data = await _model.GenerateSpeechAsync(text, _settings, cancellationToken).ConfigureAwait(false);

        if (_useCache)
        {
            SaveCachedAnswer(text, data);
        }

        values.Value[_outputKey] = data;
        return values;
    }

    byte[]? GetCachedAnswer(string prompt)
    {
        var file = Path.Combine(CACHE_DIR, $"{this.GetType().Name}.{Hash(prompt)}.ttscache");
        if (File.Exists(file))
        {
            return File.ReadAllBytes(file);
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public TTSChain UseCache(bool enabled = true)
    {
        _useCache = enabled;
        return this;
    }

    void SaveCachedAnswer(string prompt, byte[] answer)
    {
        Directory.CreateDirectory(CACHE_DIR);
        var file = Path.Combine(CACHE_DIR, $"{this.GetType().Name}.{Hash(prompt)}.ttscache");
        File.WriteAllBytes(file, answer);
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
﻿using System.Security.Cryptography;
using System.Text;
using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains;

/// <summary>
/// Speech to text chain
/// </summary>
public class STTChain : BaseStackableChain
{
    private readonly ISpeechToTextModel _model;
    private readonly SpeechToTextSettings? _settings;
    private readonly string _inputKey;
    private readonly string _outputKey;
    private bool _useCache;

    private const string CACHE_DIR = "cache";

    /// <inheritdoc />
    public STTChain(
        ISpeechToTextModel model,
        SpeechToTextSettings? settings = null,
        string inputKey = "audio",
        string outputKey = "text")
    {
        _model = model;
        _settings = settings;
        _inputKey = inputKey;
        _outputKey = outputKey;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };

    }

    string? GetCachedAnswer(byte[] prompt)
    {
        var file = Path.Combine(CACHE_DIR, $"{this.GetType().Name}.{Hash(prompt)}.sttcache");
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }
        return null;
    }

    void SaveCachedAnswer(byte[] prompt, string answer)
    {
        Directory.CreateDirectory(CACHE_DIR);
        var file = Path.Combine(CACHE_DIR, $"{this.GetType().Name}.{Hash(prompt)}.sttcache");
        File.WriteAllText(file, answer);
    }

    /// <inheritdoc />
    protected override async Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var audio = (byte[])values.Value[_inputKey];

        if (_useCache)
        {
            var cached = GetCachedAnswer(audio);
            if (cached != null)
            {

                values.Value[_outputKey] = cached;
                return values;
            }
        }

        string text = await _model.TranscribeAsync(audio, _settings, cancellationToken).ConfigureAwait(false);

        if (_useCache)
            SaveCachedAnswer(audio, text);
        values.Value[_outputKey] = text;

        return values;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public STTChain UseCache(bool enabled = true)
    {
        _useCache = enabled;
        return this;
    }

    private string Hash(byte[] prompt)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(prompt);
        string resultHex = "";
        foreach (var b in hash)
            resultHex += $"{b:x2}";
        return resultHex;

    }

}
using System.Diagnostics.CodeAnalysis;

namespace LangChain.Providers;

/// <inheritdoc/>
public class Gpt35Turbo16KModel : OpenAiModel
{
    #region Constructors

    /// <summary>
    /// Same capabilities as the standard gpt-3.5-turbo model but with 4 times the context. <br/>
    /// Max tokens: 16,384 tokens <br/>
    /// Training data: Up to Sep 2021 <br/>
    /// </summary>
    /// <param name="apiKey"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt35Turbo16KModel(string apiKey) : base(apiKey, id: OpenAiModelIds.Gpt35Turbo_16k)
    {
    }

    #endregion
}
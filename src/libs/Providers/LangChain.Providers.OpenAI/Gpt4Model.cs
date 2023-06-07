using System.Diagnostics.CodeAnalysis;

namespace LangChain.Providers;

/// <inheritdoc/>
public class Gpt4Model : OpenAiModel
{
    #region Constructors

    /// <summary>
    /// Creates OpenAI GPT4 model.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [SetsRequiredMembers]
    public Gpt4Model(string apiKey) : base(apiKey, id: OpenAI_API.Models.Model.GPT4)
    {
    }

    #endregion
}
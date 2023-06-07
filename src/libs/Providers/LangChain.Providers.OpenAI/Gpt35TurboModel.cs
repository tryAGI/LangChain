using System.Diagnostics.CodeAnalysis;

namespace LangChain.Providers;

/// <inheritdoc/>
public class Gpt35TurboModel : OpenAiModel
{
    #region Constructors

    /// <summary>
    /// Most capable GPT-3.5 model and optimized for chat at 1/10th the cost of text-davinci-003. Will be updated with the latest model iteration.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [SetsRequiredMembers]
    public Gpt35TurboModel(string apiKey) : base(apiKey, id: OpenAI_API.Models.Model.ChatGPTTurbo)
    {
    }

    #endregion
}
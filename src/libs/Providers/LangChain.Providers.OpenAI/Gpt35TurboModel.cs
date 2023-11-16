using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModel.Gpt35Turbo" />
public class Gpt35TurboModel : OpenAiModel
{
    #region Constructors

    /// <inheritdoc cref="Gpt35TurboModel" />
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt35TurboModel(string apiKey, HttpClient httpClient) : base(apiKey, id: ChatModel.Gpt35Turbo)
    {
    }

    #endregion
}
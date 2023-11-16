using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModel.Gpt35Turbo_16k" />
public class Gpt35Turbo16KModel : OpenAiModel
{
    #region Constructors

    /// <inheritdoc cref="Gpt35Turbo16KModel" />
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt35Turbo16KModel(string apiKey, HttpClient httpClient) : base(apiKey, id: ChatModel.Gpt35Turbo_16k)
    {
    }

    #endregion
}
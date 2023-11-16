using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModel.Gpt4" />
public class Gpt4Model : OpenAiModel
{
    #region Constructors

    /// <inheritdoc cref="Gpt4Model" />
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt4Model(string apiKey, HttpClient httpClient) : base(apiKey, id: ChatModel.Gpt4)
    {
    }

    #endregion
}
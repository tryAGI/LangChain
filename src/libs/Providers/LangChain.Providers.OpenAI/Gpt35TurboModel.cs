namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ModelIds.Gpt35Turbo" />
public class Gpt35TurboModel : OpenAiModel
{
    #region Constructors

    /// <inheritdoc cref="ModelIds.Gpt35Turbo" />
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt35TurboModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: ModelIds.Gpt35Turbo)
    {
    }

    #endregion
}
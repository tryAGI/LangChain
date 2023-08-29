namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ModelIds.Gpt35Turbo_16k" />
public class Gpt35Turbo16KModel : OpenAiModel
{
    #region Constructors

    /// <inheritdoc cref="ModelIds.Gpt35Turbo_16k" />
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt35Turbo16KModel(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: ModelIds.Gpt35Turbo_16k)
    {
    }

    #endregion
}
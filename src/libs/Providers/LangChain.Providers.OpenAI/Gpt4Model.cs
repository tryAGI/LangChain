namespace LangChain.Providers;

/// <inheritdoc cref="ModelIds.Gpt4" />
public class Gpt4Model : OpenAiModel
{
    #region Constructors

    /// <inheritdoc cref="ModelIds.Gpt4" />
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt4Model(string apiKey, HttpClient httpClient) : base(apiKey, httpClient, id: ModelIds.Gpt4)
    {
    }

    #endregion
}
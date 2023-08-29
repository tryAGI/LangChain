namespace LangChain.Providers.Azure;

/// <inheritdoc cref="ModelIds.Gpt35Turbo" />
public class Gpt35TurboModel : AzureModel
{
    #region Constructors

    /// <inheritdoc cref="ModelIds.Gpt35Turbo" />
    /// <param name="apiKey"></param>
    /// <param name="endpoint"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt35TurboModel(string apiKey, string endpoint, HttpClient httpClient) : base(apiKey, endpoint, httpClient, id: ModelIds.Gpt35Turbo)
    {
    }

    #endregion
}
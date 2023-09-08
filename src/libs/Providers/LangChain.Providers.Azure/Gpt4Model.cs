namespace LangChain.Providers.Azure;

/// <inheritdoc cref="ModelIds.Gpt4" />
public class Gpt4Model : AzureModel
{
    #region Constructors

    /// <inheritdoc cref="ModelIds.Gpt4" />
    /// <param name="apiKey"></param>
    /// <param name="endpoint">Specify the base server address without specifying a specific point, for example "https://myaccount.openai.azure.com/"</param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Gpt4Model(string apiKey, string endpoint, HttpClient httpClient) : base(apiKey, endpoint, httpClient, id: ModelIds.Gpt4)
    {
    }

    #endregion
}
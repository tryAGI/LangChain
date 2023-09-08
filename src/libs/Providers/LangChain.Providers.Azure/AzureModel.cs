using LangChain.Providers.OpenAI;

namespace LangChain.Providers.Azure;

/// <summary>
/// 
/// </summary>
public class AzureModel : OpenAiModel
{
    #region Constructors

    /// <summary>
    /// Wrapper around Azure large language models.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AzureModel(OpenAiConfiguration configuration, HttpClient httpClient) : base(configuration, httpClient)
    {
    }

    /// <summary>
    /// Wrapper around Azure large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <param name="endpoint">Specify the base server address without specifying a specific point, for example "https://myaccount.openai.azure.com/"</param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AzureModel(string apiKey, string endpoint, HttpClient httpClient, string id) : base(apiKey, httpClient, id)
    {
        Api.BaseUrl = endpoint;
    }

    #endregion
}
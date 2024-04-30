using Azure;
using Azure.AI.OpenAI;

namespace LangChain.Providers.Azure;

/// <summary>
/// Defines Azure OpenAI provider.
/// </summary>
public class AzureOpenAiProvider : Provider
{
    /// <summary>
    /// Azure OpenAI API Key
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// Azure OpenAI Resource URI
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Azure OpenAI Client
    /// </summary>
    [CLSCompliant(false)]
    public OpenAIClient Client { get; set; }

    public AzureOpenAiConfiguration Configurations { get; }

    #region Constructors

    /// <summary>
    /// Wrapper around Azure OpenAI
    /// </summary>
    /// <param name="apiKey">API Key</param>
    /// <param name="endpoint">Azure Open AI Resource URI</param>
    /// <exception cref="ArgumentNullException"></exception>
    public AzureOpenAiProvider(string apiKey, string endpoint) : base(id: "AzureOpenAI")
    {
        Configurations = new AzureOpenAiConfiguration();
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        Client = new OpenAIClient(new Uri(Endpoint), new AzureKeyCredential(ApiKey));
    }

    /// <summary>
    /// Wrapper around Azure OpenAI
    /// </summary>
    /// <param name="configuration">AzureOpenAIConfiguration</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public AzureOpenAiProvider(AzureOpenAiConfiguration configuration) : base(id: "AzureOpenAI")
    {
        Configurations = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        Endpoint = configuration.Endpoint ?? throw new ArgumentException("Endpoint is not defined", nameof(configuration));
        Client = new OpenAIClient(new Uri(Endpoint), new AzureKeyCredential(ApiKey));
    }

    #endregion
}
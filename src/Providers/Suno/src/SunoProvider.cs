using LangChain.Providers.Suno.Sdk;

namespace LangChain.Providers.Suno;

public class SunoProvider : Provider
{
    #region Properties

    public HttpClient HttpClient { get; private set; }

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public SunoProvider(SunoConfiguration configuration, HttpClient httpClient)
        : base(id: SunoConfiguration.SectionName)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var apiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is null.", nameof(configuration));

        HttpClient.Authorize(apiKey, configuration.StagingApiKey);
    }

    /// <inheritdoc/>
    public SunoProvider(
        string apiKey,
        HttpClient httpClient,
        string? stagingApiKey = null)
        : base(id: SunoConfiguration.SectionName)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        HttpClient.Authorize(apiKey, stagingApiKey);
    }

    #endregion
}
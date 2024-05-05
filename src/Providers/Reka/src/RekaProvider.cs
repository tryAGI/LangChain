using Reka;

namespace LangChain.Providers.Reka;

public class RekaProvider : Provider
{
    #region Properties

    public HttpClient HttpClient { get; private set; }

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public RekaProvider(RekaConfiguration configuration, HttpClient httpClient)
        : base(id: RekaConfiguration.SectionName)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var apiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is null.", nameof(configuration));
        
        HttpClient.Authorize(apiKey, baseAddress: null);
    }

    /// <inheritdoc/>
    public RekaProvider(
        string apiKey,
        HttpClient httpClient)
        : base(id: RekaConfiguration.SectionName)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        HttpClient.Authorize(apiKey, baseAddress: null);
    }

    #endregion
}
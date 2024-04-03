namespace LangChain.Providers.Google;

/// <summary>
/// </summary>
public class GoogleProvider : Provider
{
    #region Operators

    public static implicit operator GoogleProvider(string message)
    {
        return new GoogleProvider(message, new HttpClient());
    }

    #endregion

    #region Properties

    /// <summary>
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// </summary>
    public HttpClient HttpClient { get; set; }

    /// <summary>
    /// </summary>
    public GoogleConfiguration? Configuration { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GoogleProvider(GoogleConfiguration configuration, HttpClient httpClient)
        : base("Google")
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Configuration = configuration;
    }

    /// <summary>
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GoogleProvider(string apiKey, HttpClient httpClient)
        : base("Google")
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    #endregion
}
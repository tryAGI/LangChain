namespace LangChain.Providers.HuggingFace;

/// <summary>
/// 
/// </summary>
public class HuggingFaceProvider : Provider
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public HttpClient HttpClient { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public HuggingFaceApi Api { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HuggingFaceProvider(HuggingFaceConfiguration configuration, HttpClient httpClient)
        : base(id: HuggingFaceConfiguration.SectionName)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Api = new HuggingFaceApi(apiKey: ApiKey, HttpClient);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HuggingFaceProvider(string apiKey, HttpClient httpClient)
        : base(id: HuggingFaceConfiguration.SectionName)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Api = new HuggingFaceApi(apiKey: ApiKey, HttpClient);
    }

    #endregion
}
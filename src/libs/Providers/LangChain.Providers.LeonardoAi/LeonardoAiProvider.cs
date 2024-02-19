namespace LangChain.Providers.LeonardoAi;

// ReSharper disable MemberCanBePrivate.Global

/// <summary>
/// 
/// </summary>
public class LeonardoAiProvider : Provider
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string? DefaultId { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public HttpClient HttpClient { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public LeonardoAiApi Api { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around Leonardo Ai large language models.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LeonardoAiProvider(LeonardoAiConfiguration configuration, HttpClient httpClient)
        : base(id: LeonardoAiConfiguration.SectionName)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        DefaultId = configuration.ModelId ?? throw new ArgumentException("ModelId is not defined", nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        Api = new LeonardoAiApi(apiKey: ApiKey, HttpClient);
    }

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <param name="defaultId"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LeonardoAiProvider(
        string apiKey,
        HttpClient httpClient,
        string? defaultId = null)
        : base(id: LeonardoAiConfiguration.SectionName)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        DefaultId = defaultId;

        Api = new LeonardoAiApi(apiKey: ApiKey, HttpClient);
    }

    #endregion
}
namespace LangChain.Providers;

// ReSharper disable MemberCanBePrivate.Global

/// <summary>
/// https://openai.com/
/// </summary>
public partial class LeonardoAiModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; init; }
    
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
    public LeonardoAiModel(LeonardoAiConfiguration configuration, HttpClient httpClient)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        Id = configuration.ModelId ?? throw new ArgumentException("ModelId is not defined", nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        
        Api = new LeonardoAiApi(apiKey: ApiKey, HttpClient);
    }
    
    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LeonardoAiModel(string apiKey, HttpClient httpClient, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Id = id ?? throw new ArgumentNullException(nameof(id));
        
        Api = new LeonardoAiApi(apiKey: ApiKey, HttpClient);
    }

    #endregion
}
namespace LangChain.Providers;

// ReSharper disable MemberCanBePrivate.Global

/// <summary>
/// https://openai.com/
/// </summary>
public partial class AnyscaleModel :
    IPaidLargeLanguageModel,
    IModelWithUniqueUserIdentifier
{
    #region Fields

    private readonly object _usageLock = new();

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string ApiKey { get; init; }

    /// <inheritdoc cref="IChatModel.TotalUsage"/>
    public Usage TotalUsage { get; private set; }

    /// <inheritdoc/>
    public string User { get; set; } = string.Empty;

    /// <inheritdoc/>
    public int ContextLength => ApiHelpers.CalculateContextLength(Id);

    /// <summary>
    /// 
    /// </summary>
    public HttpClient HttpClient { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public AnyscaleApi Api { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AnyscaleModel(AnyscaleConfiguration configuration, HttpClient httpClient)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        Id = configuration.ModelId ?? throw new ArgumentException("ModelId is not defined", nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        Encoding = Tiktoken.Encoding.ForModel(Id);
        Api = new AnyscaleApi(apiKey: ApiKey, HttpClient);
    }

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AnyscaleModel(string apiKey, HttpClient httpClient, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Id = id ?? throw new ArgumentNullException(nameof(id));

        Encoding = Tiktoken.Encoding.ForModel(Id);
        Api = new AnyscaleApi(apiKey: ApiKey, HttpClient);
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public double CalculatePriceInUsd(int promptTokens, int completionTokens)
    {
        return ApiHelpers.CalculatePriceInUsd(
            modelId: Id,
            completionTokens: completionTokens,
            promptTokens: promptTokens);
    }

    #endregion
}
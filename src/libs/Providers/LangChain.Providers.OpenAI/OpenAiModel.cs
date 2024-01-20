using OpenAI;
using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

// ReSharper disable MemberCanBePrivate.Global

/// <summary>
/// https://openai.com/
/// </summary>
public partial class OpenAiModel :
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

    /// <summary>
    /// Sampling temperature
    /// </summary>
    public double Temperature { get; set; } = 1.0;

    /// <inheritdoc/>
    public int ContextLength => ContextLengths.Get(Id);

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public OpenAIClient Api { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OpenAiModel(OpenAiConfiguration configuration)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        Id = configuration.ModelId ?? throw new ArgumentException("ModelId is not defined", nameof(configuration));
        EmbeddingModelId = configuration.EmbeddingModelId ?? throw new ArgumentException("EmbeddingModelId is not defined", nameof(configuration));

        Temperature = configuration.Temperature;
        Encoding = Tiktoken.Encoding.TryForModel(Id) ?? Tiktoken.Encoding.Get(Tiktoken.Encodings.Cl100KBase);
        Api = new OpenAIClient(ApiKey);
        if (configuration.Endpoint != null &&
            !string.IsNullOrWhiteSpace(configuration.Endpoint))
        {
            Api = new OpenAIClient(ApiKey, new OpenAIClientSettings(domain: configuration.Endpoint));
        }
    }
    
    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OpenAiModel(string apiKey, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Id = id ?? throw new ArgumentNullException(nameof(id));

        Encoding = Tiktoken.Encoding.TryForModel(Id) ?? Tiktoken.Encoding.Get(Tiktoken.Encodings.Cl100KBase);
        Api = new OpenAIClient(ApiKey);
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public double CalculatePriceInUsd(int inputTokens, int outputTokens)
    {
        return ChatPrices.TryGet(
            model: new ChatModel(Id),
            outputTokens: outputTokens,
            inputTokens: inputTokens) ?? 0.0;
    }

    #endregion
}
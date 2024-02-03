using OpenAI;

namespace LangChain.Providers.OpenAI;

/// <summary>
/// https://openai.com/
/// </summary>
public class OpenAiProvider : Provider
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public OpenAIClient Api { get; private set; }

    #endregion

    #region Constructors

    public OpenAiProvider(OpenAiConfiguration configuration)
        : base(id: OpenAiConfiguration.SectionName)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));

        ChatSettings = new OpenAiChatSettings
        {
            //StopSequences = configuration.StopSequences,
            Temperature = configuration.Temperature,
            User = string.Empty,
        };
        Api = new OpenAIClient(ApiKey);
        if (configuration.Endpoint != null &&
            !string.IsNullOrWhiteSpace(configuration.Endpoint))
        {
            Api = new OpenAIClient(ApiKey, new OpenAIClientSettings(domain: configuration.Endpoint));
        }
    }
    
    public OpenAiProvider(
        string apiKey,
        string? customEndpoint = null)
        : base(id: OpenAiConfiguration.SectionName)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

        Api = customEndpoint != null
            ? new OpenAIClient(ApiKey, new OpenAIClientSettings(domain: customEndpoint))
            : new OpenAIClient(ApiKey);
    }

    #endregion
}
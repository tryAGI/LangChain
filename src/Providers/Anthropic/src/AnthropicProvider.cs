namespace LangChain.Providers.Anthropic;

/// <summary>
/// 
/// </summary>
public class AnthropicProvider(
    string apiKey,
    HttpClient? httpClient = null)
    : Provider(id: AnthropicConfiguration.SectionName)
{
    #region Properties
    
    [CLSCompliant(false)]
    public AnthropicApi Api { get; } = new(apiKey, httpClient ?? new HttpClient());
    
    #endregion

    #region Static methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static AnthropicProvider FromConfiguration(AnthropicConfiguration configuration, HttpClient httpClient)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        
        return new AnthropicProvider(configuration.ApiKey, httpClient);
    }

    #endregion
}
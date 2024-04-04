using Anthropic.SDK;

namespace LangChain.Providers.Anthropic;

/// <summary>
/// 
/// </summary>
public class AnthropicProvider(
    string apiKey,
    IHttpClientFactory? httpClientFactory = null)
    : Provider(id: AnthropicConfiguration.SectionName)
{
    #region Properties
    
    [CLSCompliant(false)]
    public AnthropicClient Api { get; } = new(apiKey){HttpClientFactory = httpClientFactory };
    
    #endregion

    #region Static methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClientFactory"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static AnthropicProvider FromConfiguration(AnthropicConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        
        return new AnthropicProvider(configuration.ApiKey, httpClientFactory);
    }

    #endregion
}
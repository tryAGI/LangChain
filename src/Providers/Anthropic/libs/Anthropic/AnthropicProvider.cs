namespace LangChain.Providers.Anthropic;

/// <summary>
/// </summary>
public class AnthropicProvider(
    string apiKey)
    : Provider(AnthropicConfiguration.SectionName)
{
    #region Properties

    [CLSCompliant(false)] public AnthropicClient Api { get; } = new(apiKey);

    #endregion

    #region Static methods

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static AnthropicProvider FromConfiguration(AnthropicConfiguration configuration)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        return new AnthropicProvider(configuration.ApiKey);
    }

    #endregion
}
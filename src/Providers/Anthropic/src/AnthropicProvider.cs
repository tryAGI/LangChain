namespace LangChain.Providers.Anthropic;

/// <inheritdoc />
public class AnthropicProvider : Provider
{
    public AnthropicApi Api { get; } = new();

    public AnthropicProvider(string apiKey) : base(id: AnthropicConfiguration.SectionName)
    {
        Api.AuthorizeUsingApiKey(apiKey);
    }

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
}
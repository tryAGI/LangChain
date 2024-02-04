namespace LangChain.Providers.Anthropic.Predefined;

/// <inheritdoc cref="ModelIds.Claude" />
public class ClaudeModel(string apiKey, HttpClient? httpClient = null)
    : AnthropicModel(new AnthropicProvider(apiKey, httpClient), id: ModelIds.Claude);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class ClaudeInstantModel(string apiKey, HttpClient? httpClient = null)
    : AnthropicModel(new AnthropicProvider(apiKey, httpClient), id: ModelIds.ClaudeInstant);
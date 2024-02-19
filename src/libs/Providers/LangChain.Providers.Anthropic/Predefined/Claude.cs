namespace LangChain.Providers.Anthropic.Predefined;

/// <inheritdoc cref="ModelIds.Claude" />
public class ClaudeModel(AnthropicProvider provider)
    : AnthropicModel(provider, id: ModelIds.Claude);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class ClaudeInstantModel(AnthropicProvider provider)
    : AnthropicModel(provider, id: ModelIds.ClaudeInstant);
using Anthropic.SDK.Constants;

namespace LangChain.Providers.Anthropic.Predefined;

///// <inheritdoc cref="ModelIds.Claude" />
//public class ClaudeModel(AnthropicProvider provider)
//    : AnthropicModel(provider, id: ModelIds.Claude);

///// <inheritdoc cref="ModelIds.ClaudeInstant" />
//public class ClaudeInstantModel(AnthropicProvider provider)
//    : AnthropicModel(provider, id: ModelIds.ClaudeInstant);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class Claude3Haiku(AnthropicProvider provider)
    : AnthropicModel(provider, id: AnthropicModels.Claude3Haiku);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class Claude3Sonnet(AnthropicProvider provider)
    : AnthropicModel(provider, id: AnthropicModels.Claude3Sonnet);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class Claude3Opus(AnthropicProvider provider)
    : AnthropicModel(provider, id: AnthropicModels.Claude3Opus);
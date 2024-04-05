using Anthropic.SDK.Constants;

namespace LangChain.Providers.Anthropic.Predefined;

/// <inheritdoc cref="ModelIds.Claude" />
public class ClaudeModelV21(AnthropicProvider provider)
    : AnthropicModel(provider, id: AnthropicModels.Claude_v2_1);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class ClaudeInstantModel(AnthropicProvider provider)
    : AnthropicModel(provider, id: AnthropicModels.ClaudeInstant_v1_2);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class Claude3Haiku(AnthropicProvider provider)
    : AnthropicModel(provider, AnthropicModels.Claude3Haiku);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class Claude3Sonnet(AnthropicProvider provider)
    : AnthropicModel(provider, AnthropicModels.Claude3Sonnet);

/// <inheritdoc cref="ModelIds.ClaudeInstant" />
public class Claude3Opus(AnthropicProvider provider)
    : AnthropicModel(provider, AnthropicModels.Claude3Opus);
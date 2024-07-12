using System.Diagnostics.CodeAnalysis;
using Anthropic.SDK.Constants;

namespace LangChain.Providers.Anthropic.Predefined;

/// <summary>
/// Updated version of Claude 2 with improved accuracy
/// Max Tokens: 200K
/// </summary>
[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
public class ClaudeModelV21(AnthropicProvider provider)
    : AnthropicModel(provider, id: AnthropicModels.Claude_v2_1);

/// <summary>
/// Low-latency, high throughout. <br/>
/// Max tokens: 100,000 tokens <br/>
/// Training data: Up to February 2023 <br/>
/// </summary>
[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
public class ClaudeInstantModel(AnthropicProvider provider)
    : AnthropicModel(provider, id: AnthropicModels.ClaudeInstant_v1_2);

/// <summary>
/// Fastest and most compact model for near-instant responsiveness
/// Max Tokens: 200K
/// </summary>
[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
public class Claude3Haiku(AnthropicProvider provider)
    : AnthropicModel(provider, AnthropicModels.Claude3Haiku);

/// <summary>
/// Ideal balance of intelligence and speed for enterprise workloads
/// Max Tokens: 200K
/// </summary>
[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
public class Claude3Sonnet(AnthropicProvider provider)
    : AnthropicModel(provider, AnthropicModels.Claude3Sonnet);

/// <summary>
/// Most powerful model for highly complex tasks <br/>
/// Max Tokens: 200K
/// </summary>
[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
public class Claude3Opus(AnthropicProvider provider)
    : AnthropicModel(provider, AnthropicModels.Claude3Opus);

/// <summary>
/// Ideal balance of intelligence and speed for enterprise workloads
/// Max Tokens: 200K
/// </summary>
[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
public class Claude35Sonnet(AnthropicProvider provider)
    : AnthropicModel(provider, AnthropicModels.Claude35Sonnet);
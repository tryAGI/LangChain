using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Anthropic;

/// <inheritdoc />
public class ClaudeInstantV1Model(RegionEndpoint? region = null)
    : AnthropicClaudeChatModel(new BedrockProvider(region), id: "anthropic.claude-instant-v1");

/// <inheritdoc />
public class ClaudeV1Model(RegionEndpoint? region = null)
    : AnthropicClaudeChatModel(new BedrockProvider(region), id: "anthropic.claude-v1");

/// <inheritdoc />
public class ClaudeV2Model(RegionEndpoint? region = null)
    : AnthropicClaudeChatModel(new BedrockProvider(region), id: "anthropic.claude-v2");

/// <inheritdoc />
public class ClaudeV21Model(RegionEndpoint? region = null)
    : AnthropicClaudeChatModel(new BedrockProvider(region), id: "anthropic.claude-v2:1");
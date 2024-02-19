// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Anthropic;

/// <inheritdoc />
public class ClaudeInstantV1Model(BedrockProvider provider)
    : AnthropicClaudeChatModel(provider, id: "anthropic.claude-instant-v1");

/// <inheritdoc />
public class ClaudeV1Model(BedrockProvider provider)
    : AnthropicClaudeChatModel(provider, id: "anthropic.claude-v1");

/// <inheritdoc />
public class ClaudeV2Model(BedrockProvider provider)
    : AnthropicClaudeChatModel(provider, id: "anthropic.claude-v2");

/// <inheritdoc />
public class ClaudeV21Model(BedrockProvider provider)
    : AnthropicClaudeChatModel(provider, id: "anthropic.claude-v2:1");
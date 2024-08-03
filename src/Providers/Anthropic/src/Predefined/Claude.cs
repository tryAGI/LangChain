namespace LangChain.Providers.Anthropic.Predefined;

/// <summary>
/// Updated version of Claude 2 with improved accuracy
/// Max Tokens: 200K
/// </summary>
public class ClaudeModelV21(AnthropicProvider provider)
    : AnthropicModel(provider, id: CreateMessageRequestModel.Claude21.ToValueString());

/// <summary>
/// Low-latency, high throughout. <br/>
/// Max tokens: 100,000 tokens <br/>
/// Training data: Up to February 2023 <br/>
/// </summary>
public class ClaudeInstantModel(AnthropicProvider provider)
    : AnthropicModel(provider, id: CreateMessageRequestModel.ClaudeInstant12.ToValueString());

/// <summary>
/// Fastest and most compact model for near-instant responsiveness
/// Max Tokens: 200K
/// </summary>
public class Claude3Haiku(AnthropicProvider provider)
    : AnthropicModel(provider, CreateMessageRequestModel.Claude3Haiku20240307.ToValueString());

/// <summary>
/// Ideal balance of intelligence and speed for enterprise workloads
/// Max Tokens: 200K
/// </summary>
public class Claude3Sonnet(AnthropicProvider provider)
    : AnthropicModel(provider, CreateMessageRequestModel.Claude3Sonnet20240229.ToValueString());

/// <summary>
/// Most powerful model for highly complex tasks <br/>
/// Max Tokens: 200K
/// </summary>
public class Claude3Opus(AnthropicProvider provider)
    : AnthropicModel(provider, CreateMessageRequestModel.Claude3Opus20240229.ToValueString());

/// <summary>
/// Ideal balance of intelligence and speed for enterprise workloads
/// Max Tokens: 200K
/// </summary>
public class Claude35Sonnet(AnthropicProvider provider)
    : AnthropicModel(provider, CreateMessageRequestModel.Claude35Sonnet20240620.ToValueString());
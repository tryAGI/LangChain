// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Mistral;

/// <inheritdoc />
public class Mistral7BInstruct(BedrockProvider provider)
    : MistralInstructChatModel(provider, id: "mistral.mistral-7b-instruct-v0:2");

/// <inheritdoc />
public class Mistral8x7BInstruct(BedrockProvider provider)
    : MistralInstructChatModel(provider, id: "mistral.mixtral-8x7b-instruct-v0:1");

/// <inheritdoc />
public class MistralLarge(BedrockProvider provider)
    : MistralInstructChatModel(provider, id: "mistral.mistral-large-2402-v1:0");
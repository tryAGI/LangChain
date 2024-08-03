// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Mistral;

/// <inheritdoc />
public class Mistral7BInstruct(BedrockProvider provider)
    : MistralModel(provider, id: "mistral.mistral-7b-instruct-v0:2");

/// <inheritdoc />
public class Mistral8x7BInstruct(BedrockProvider provider)
    : MistralModel(provider, id: "mistral.mixtral-8x7b-instruct-v0:1");

/// <inheritdoc />
/// DEPRECATE
public class MistralLarge(BedrockProvider provider)
    : MistralModel(provider, id: "mistral.mistral-large-2402-v1:0");

/// <inheritdoc />
public class MistralLarge2402(BedrockProvider provider)
    : MistralModel(provider, id: "mistral.mistral-large-2402-v1:0");

/// <inheritdoc />
public class MistralLarge2407(BedrockProvider provider)
    : MistralModel(provider, id: "mistral.mistral-large-2407-v1:0");
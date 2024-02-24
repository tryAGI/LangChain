// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Ai21Labs;

/// <inheritdoc />
public class Jurassic2MidModel(BedrockProvider provider)
    : Ai21LabsJurassic2ChatModel(provider, id: "ai21.j2-mid-v1");

/// <inheritdoc />
public class Jurassic2UltraModel(BedrockProvider provider)
    : Ai21LabsJurassic2ChatModel(provider, id: "ai21.j2-ultra-v1");
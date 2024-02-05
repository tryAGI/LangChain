using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Ai21Labs;

/// <inheritdoc />
public class Jurassic2MidModel(RegionEndpoint? region = null)
    : Ai21LabsJurassic2ChatModel(new BedrockProvider(region), id: "ai21.j2-mid-v1");

/// <inheritdoc />
public class Jurassic2UltraModel(RegionEndpoint? region = null)
    : Ai21LabsJurassic2ChatModel(new BedrockProvider(region), id: "ai21.j2-ultra-v1");
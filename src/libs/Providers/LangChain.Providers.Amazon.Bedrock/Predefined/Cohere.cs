using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Cohere;

/// <inheritdoc />
public class CommandTextV14Model(RegionEndpoint? region = null)
    : CohereCommandChatModel(new BedrockProvider(region), id: "cohere.command-text-v14");

/// <inheritdoc />
public class CommandLightTextV14Model(RegionEndpoint? region = null)
    : CohereCommandChatModel(new BedrockProvider(region), id: "cohere.command-light-text-v14");

/// <inheritdoc />
public class EmbedEnglishV3Model(RegionEndpoint? region = null)
    : CohereEmbeddingModel(new BedrockProvider(region), id: "cohere.embed-english-v3");

/// <inheritdoc />
public class EmbedMultilingualV3Model(RegionEndpoint? region = null)
    : CohereEmbeddingModel(new BedrockProvider(region), id: "cohere.embed-multilingual-v3");

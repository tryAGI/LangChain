using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Cohere;

/// <inheritdoc />
public abstract class CommandTextV14Model(RegionEndpoint? region = null)
    : CohereCommandChatModel(new BedrockProvider(region), id: "cohere.command-text-v14");

/// <inheritdoc />
public abstract class CommandLightTextV14Model(RegionEndpoint? region = null)
    : CohereCommandChatModel(new BedrockProvider(region), id: "cohere.command-light-text-v14");

/// <inheritdoc />
public abstract class EmbedEnglishV3Model(RegionEndpoint? region = null)
    : CohereEmbeddingModel(new BedrockProvider(region), id: "cohere.embed-english-v3");

/// <inheritdoc />
public abstract class EmbedMultilingualV3Model(RegionEndpoint? region = null)
    : CohereEmbeddingModel(new BedrockProvider(region), id: "cohere.embed-multilingual-v3");

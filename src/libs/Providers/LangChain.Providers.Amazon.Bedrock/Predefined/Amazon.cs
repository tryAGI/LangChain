using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;

/// <inheritdoc />
public class TitanTextExpressV1Model(RegionEndpoint? region = null)
    : AmazonTitanChatModel(new BedrockProvider(region), id: "amazon.titan-text-express-v1");

/// <inheritdoc />
public class TitanTextLiteV1Model(RegionEndpoint? region = null)
    : AmazonTitanChatModel(new BedrockProvider(region), id: "amazon.titan-text-lite-v1");

/// <inheritdoc />
public class TitanEmbedTextV1Model(RegionEndpoint? region = null)
    : AmazonTitanEmbeddingModel(new BedrockProvider(region), id: "amazon.titan-embed-text-v1");

/// <inheritdoc />
public class TitanEmbedImageV1Model(RegionEndpoint? region = null)
    : AmazonTitanImageEmbeddingModel(new BedrockProvider(region), id: "amazon.titan-embed-image-v1");

/// <inheritdoc />
public class TitanImageGeneratorV1Model(RegionEndpoint? region = null)
    : AmazonTitanImageGenerationModel(new BedrockProvider(region), id: "amazon.titan-image-generator-v1");

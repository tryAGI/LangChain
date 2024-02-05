using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;

// TODO
/// <inheritdoc />
public abstract class TitanTextExpressV1Model()
    : ChatModel(id: "amazon.titan-text-express-v1");

// TODO
/// <inheritdoc />
public abstract class TitanTextLiteV1Model()
    : ChatModel(id: "amazon.titan-text-lite-v1");

/// <inheritdoc />
public class TitanEmbedTextV1Model(RegionEndpoint? region = null)
    : AmazonTitanEmbeddingModel(new BedrockProvider(region), id: "amazon.titan-embed-text-v1");

// TODO
/// <inheritdoc />
public abstract class TitanEmbedImageV1Model()
    : Model(id: "amazon.titan-embed-image-v1");

// TODO
/// <inheritdoc />
public abstract class TitanImageGeneratorV1Model()
    : ImageGenerationModel(id: "amazon.titan-image-generator-v1");
// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Cohere;

/// <inheritdoc />
public class CommandTextV14Model(BedrockProvider provider)
    : CohereCommandChatModel(provider, id: "cohere.command-text-v14");

/// <inheritdoc />
public class CommandLightTextV14Model(BedrockProvider provider)
    : CohereCommandChatModel(provider, id: "cohere.command-light-text-v14");

/// <inheritdoc />
public class EmbedEnglishV3Model(BedrockProvider provider)
    : CohereEmbeddingModel(provider, id: "cohere.embed-english-v3");

/// <inheritdoc />
public class EmbedMultilingualV3Model(BedrockProvider provider)
    : CohereEmbeddingModel(provider, id: "cohere.embed-multilingual-v3");

/// <inheritdoc />
public class CommandRPlusModel(BedrockProvider provider)
    : CohereCommandRModel(provider, id: "cohere.command-r-plus-v1:0");

/// <inheritdoc />
public class CommandRModel(BedrockProvider provider)
    : CohereCommandRModel(provider, id: "cohere.command-r-v1:0");

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Cohere;

// TODO
/// <inheritdoc />
public abstract class CommandTextV14Model()
    : ChatModel(id: "cohere.command-text-v14");

// TODO
/// <inheritdoc />
public abstract class CommandLightTextV14Model()
    : ChatModel(id: "cohere.command-light-text-v14");

// TODO
/// <inheritdoc />
public abstract class EmbedEnglishV3Model()
    : Model(id: "cohere.embed-english-v3");

// TODO
/// <inheritdoc />
public abstract class EmbedMultilingualV3Model()
    : Model(id: "cohere.embed-multilingual-v3");

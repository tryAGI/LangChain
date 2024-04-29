// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Meta;


/// <inheritdoc />
public class Llama2With13BModel(BedrockProvider provider)
    : MetaLlamaChatModel(provider, id: "meta.llama2-13b-v1");

/// <inheritdoc />
public class Llama2With70BModel(BedrockProvider provider)
    : MetaLlamaChatModel(provider, id: "meta.llama2-70b-v1");

/// <inheritdoc />
public class Llama2WithChat13BModel(BedrockProvider provider)
    : MetaLlamaChatModel(provider, id: "meta.llama2-13b-chat-v1");

/// <inheritdoc />
public class Llama2WithChat70BModel(BedrockProvider provider)
    : MetaLlamaChatModel(provider, id: "meta.llama2-70b-chat-v1");

/// <inheritdoc />
public class Llama3With8BInstructBModel(BedrockProvider provider)
    : MetaLlamaChatModel(provider, id: "meta.llama3-8b-instruct-v1:0");

/// <inheritdoc />
public class Llama3With70BInstructBModel(BedrockProvider provider)
    : MetaLlamaChatModel(provider, id: "meta.llama3-8b-instruct-v1:0");
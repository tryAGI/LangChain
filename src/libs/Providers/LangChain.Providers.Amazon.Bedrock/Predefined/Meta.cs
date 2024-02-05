using Amazon;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Meta;

/// <inheritdoc />
public class Llama2With13BModel(RegionEndpoint? region = null)
    : MetaLlama2ChatModel(new BedrockProvider(region), id: "meta.llama2-13b-chat-v1");

/// <inheritdoc />
public class Llama2With70BModel(RegionEndpoint? region = null)
    : MetaLlama2ChatModel(new BedrockProvider(region), id: "meta.llama2-70b-chat-v1");
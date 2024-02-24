// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock.Predefined.Meta;

/// <inheritdoc />
public class Llama2With13BModel(BedrockProvider provider)
    : MetaLlama2ChatModel(provider, id: "meta.llama2-13b-chat-v1");

/// <inheritdoc />
public class Llama2With70BModel(BedrockProvider provider)
    : MetaLlama2ChatModel(provider, id: "meta.llama2-70b-chat-v1");

//public static string MetaLlama213B = "meta.llama2-13b-v1";  //TODO i'm guessing the model id
//public static string MetaLlama270B = "meta.llama2-70b-v1";  //TODO i'm guessing the model id

namespace LangChain.Providers.DeepSeek.Predefined;

/// <inheritdoc cref="DeepSeekModels.DeepSeekChat" />
public class DeepSeekChatModel(DeepSeekProvider provider)
    : DeepSeekModel(provider, DeepSeekModels.DeepSeekChat);

/// <inheritdoc cref="DeepSeekModels.DeepSeekChat" />
public class DeepSeekCoderModel(DeepSeekProvider provider)
    : DeepSeekModel(provider, DeepSeekModels.DeepSeekCoder);
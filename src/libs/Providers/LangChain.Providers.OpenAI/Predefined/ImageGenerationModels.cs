using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="ImageModels.DallE2" />
public class DallE2Model(OpenAiProvider provider)
    : OpenAiTextToSpeechModel(provider, id: ImageModels.DallE2);

/// <inheritdoc cref="ImageModels.DallE3" />
public class DallE3Model(OpenAiProvider provider)
    : OpenAiTextToSpeechModel(provider, id: ImageModels.DallE3);
namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="CreateImageRequestModel.DallE2" />
public class DallE2Model(OpenAiProvider provider)
    : OpenAiTextToImageModel(provider, id: CreateImageRequestModel.DallE2);

/// <inheritdoc cref="CreateImageRequestModel.DallE3" />
public class DallE3Model(OpenAiProvider provider)
    : OpenAiTextToImageModel(provider, id: CreateImageRequestModel.DallE3);
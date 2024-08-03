namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="CreateModerationRequestModel.TextModerationLatest" />
public class LatestModerationModel(OpenAiProvider provider)
    : OpenAiModerationModel(provider, id: CreateModerationRequestModel.TextModerationLatest);

/// <inheritdoc cref="CreateModerationRequestModel.TextModerationStable" />
public class StableModerationModel(OpenAiProvider provider)
    : OpenAiModerationModel(provider, id: CreateModerationRequestModel.TextModerationStable);
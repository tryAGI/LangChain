using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="ModerationModels.Latest" />
public class LatestModerationModel(OpenAiProvider provider)
    : OpenAiModerationModel(provider, id: ModerationModels.Latest);

/// <inheritdoc cref="ModerationModels.Stable" />
public class StableModerationModel(OpenAiProvider provider)
    : OpenAiModerationModel(provider, id: ModerationModels.Stable);
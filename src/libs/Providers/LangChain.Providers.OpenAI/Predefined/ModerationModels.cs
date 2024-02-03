using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="ModerationModels.Latest" />
public class LatestModerationModel(string apiKey)
    : OpenAiModerationModel(new OpenAiProvider(apiKey), id: ModerationModels.Latest);

/// <inheritdoc cref="ModerationModels.Stable" />
public class StableModerationModel(string apiKey)
    : OpenAiModerationModel(new OpenAiProvider(apiKey), id: ModerationModels.Stable);
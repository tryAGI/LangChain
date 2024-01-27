using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModels.Gpt35Turbo" />
public class Gpt35TurboModel(string apiKey)
    : OpenAiModel(apiKey, id: ChatModels.Gpt35Turbo);
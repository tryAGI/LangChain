using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModels.Gpt35Turbo_16k" />
public class Gpt35Turbo16KModel(string apiKey)
    : OpenAiModel(apiKey, id: ChatModels.Gpt35Turbo_16k);
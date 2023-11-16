using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModel.Gpt35Turbo" />
public class Gpt35TurboModel(string apiKey)
    : OpenAiModel(apiKey, id: ChatModel.Gpt35Turbo);
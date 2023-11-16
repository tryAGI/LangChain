using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModel.Gpt35Turbo_16k" />
public class Gpt35Turbo16KModel(string apiKey)
    : OpenAiModel(apiKey, id: ChatModel.Gpt35Turbo_16k);
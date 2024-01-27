using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModels.Gpt4" />
public class Gpt4Model(string apiKey)
    : OpenAiModel(apiKey, id: ChatModels.Gpt4);
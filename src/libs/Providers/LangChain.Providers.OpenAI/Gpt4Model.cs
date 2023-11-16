using OpenAI.Constants;

namespace LangChain.Providers.OpenAI;

/// <inheritdoc cref="ChatModel.Gpt4" />
public class Gpt4Model(string apiKey)
    : OpenAiModel(apiKey, id: ChatModel.Gpt4);
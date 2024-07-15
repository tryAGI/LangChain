namespace LangChain.Providers.OpenAI.Predefined;

// Now we only predefine the models with alias.

/// <inheritdoc cref="CreateChatCompletionRequestModel.Gpt35Turbo" />
public class Gpt35TurboModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: CreateChatCompletionRequestModel.Gpt35Turbo);

/// <inheritdoc cref="CreateChatCompletionRequestModel.Gpt4" />
public class Gpt4Model(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: CreateChatCompletionRequestModel.Gpt4);

/// <inheritdoc cref="CreateChatCompletionRequestModel.Gpt432k" />
public class Gpt4With32KModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: CreateChatCompletionRequestModel.Gpt432k);

/// <inheritdoc cref="CreateChatCompletionRequestModel.Gpt4TurboPreview" />
public class Gpt4TurboPreviewModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: CreateChatCompletionRequestModel.Gpt4TurboPreview);

/// <inheritdoc cref="CreateChatCompletionRequestModel.Gpt4VisionPreview" />
public class Gpt4VisionPreviewModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: CreateChatCompletionRequestModel.Gpt4VisionPreview);

/// <inheritdoc cref="CreateChatCompletionRequestModel.Gpt4Turbo" />
public class Gpt4TurboModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: CreateChatCompletionRequestModel.Gpt4Turbo);

/// <inheritdoc cref="CreateChatCompletionRequestModel.Gpt4o" />
public class Gpt4OmniModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: CreateChatCompletionRequestModel.Gpt4o);
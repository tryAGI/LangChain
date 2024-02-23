using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

// Now we only predefine the models with alias, not the models with fixed id.

/// <inheritdoc cref="ChatModels.Gpt35Turbo_16k" />
public class Gpt35Turbo16KModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt35Turbo_16k);
        
/// <inheritdoc cref="ChatModels.Gpt35Turbo" />
public class Gpt35TurboModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt35Turbo);

/// <inheritdoc cref="ChatModels.Gpt4" />
public class Gpt4Model(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt4);

/// <inheritdoc cref="ChatModels.Gpt4_32k" />
public class Gpt4With32KModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt4_32k);

/// <inheritdoc cref="ChatModels.Gpt35TurboInstruct" />
public class Gpt35TurboInstructModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt35TurboInstruct);

/// <inheritdoc cref="ChatModels.Gpt4TurboPreview" />
public class Gpt4TurboPreviewModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt4TurboPreview);

/// <inheritdoc cref="ChatModels.Gpt4VisionPreview" />
public class Gpt4VisionPreviewModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt4VisionPreview);

/// <inheritdoc cref="ChatModels.Gpt35Turbo_16k" />
public class Gpt35TurboWith16KModel(OpenAiProvider provider)
    : OpenAiChatModel(provider, id: ChatModels.Gpt35Turbo_16k);
using GenerativeAI.Models;

namespace LangChain.Providers.Google.Predefined;

/// <inheritdoc cref="GoogleAIModels.GeminiPro" />
public class GeminiProModel(GoogleProvider provider)
    : GoogleChatModel(
        provider,
        GoogleAIModels.GeminiPro);

/// <inheritdoc cref="GoogleAIModels.GeminiProVision" />
public class GeminiProVisionModel(GoogleProvider provider)
    : GoogleChatModel(
        provider,
        GoogleAIModels.GeminiProVision);
using GenerativeAI.Models;

namespace LangChain.Providers.Google.Predefined;

/// <inheritdoc cref="GoogleAIModels.GeminiPro" />
public class GeminiProModel(GoogleProvider provider)
    : GoogleChatModel(
        provider,
        GoogleAIModels.GeminiPro, 32 * 1024, 0.5 * 0.000001, 1.5 * 0.000001);

/// <inheritdoc cref="GoogleAIModels.GeminiProVision" />
public class GeminiProVisionModel(GoogleProvider provider)
    : GoogleChatModel(
        provider,
        GoogleAIModels.GeminiProVision, 32 * 1024, 0.5 * 0.000001, 1.5 * 0.000001);

/// <inheritdoc cref="GoogleAIModels.GeminiProVision" />
public class Gemini15FlashModel(GoogleProvider provider)
    : GoogleChatModel(
        provider,
        GoogleAIModels.Gemini15Flash, 1024 * 1024, 0.35 * 0.000001, 1.05 * 0.000001, 0.70 * 0.000001, 2.1 * 0.000001);

/// <inheritdoc cref="GoogleAIModels.GeminiProVision" />
public class Gemini15ProModel(GoogleProvider provider)
    : GoogleChatModel(
        provider,
        GoogleAIModels.Gemini15Pro, 2 * 1024 * 1024, 3.5 * 0.000001, 10.50 * 0.000001, 7.0 * 0.000001, 21.00 * 0.000001);

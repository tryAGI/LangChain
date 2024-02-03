using GenerativeAI.Models;

namespace LangChain.Providers.Google.Predefined;

/// <inheritdoc cref="GoogleAIModels.GeminiPro" />
public class GeminiProModel(string apiKey, HttpClient httpClient)
    : GoogleChatModel(
        provider: new GoogleProvider(apiKey, httpClient),
        id: GoogleAIModels.GeminiPro);

/// <inheritdoc cref="GoogleAIModels.GeminiProVision" />
public class GeminiProVisionModel(string apiKey, HttpClient httpClient)
    : GoogleChatModel(
        provider: new GoogleProvider(apiKey, httpClient),
        id: GoogleAIModels.GeminiProVision);
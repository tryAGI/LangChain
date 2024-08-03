namespace LangChain.Providers.Google.VertexAI.Predefined
{
    public class Gemini15ProChatModel(VertexAIProvider provider)
    : VertexAIChatModel(provider, "gemini-1.5-pro");

    public class Gemini15FlashChatModel(VertexAIProvider provider)
    : VertexAIChatModel(provider, "gemini-1.5-flash");

    public class Gemini1ProChatModel(VertexAIProvider provider)
    : VertexAIChatModel(provider, "gemini-1.0-pro");

    public class Gemini15ProImageToTextModel(VertexAIProvider provider, string mimeType = "image/png")
        : VertexAIImageToTextModel(provider, "gemini-1.5-pro", mimeType);

    public class Gemini15FlashImageToTextModel(VertexAIProvider provider, string mimeType = "image/png")
    : VertexAIImageToTextModel(provider, "gemini-1.5-flash", mimeType);
}

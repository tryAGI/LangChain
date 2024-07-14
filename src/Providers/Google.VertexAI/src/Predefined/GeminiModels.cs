namespace LangChain.Providers.Google.VertexAI.Predefined
{
    public class Gemini15ProModel(VertexAIProvider provider)
    : VertexAIChatModel(provider, "gemini-1.5-pro");

    public class Gemini15FlashModel(VertexAIProvider provider)
    : VertexAIChatModel(provider, "gemini-1.5-flash");

    public class Gemini1ProModel(VertexAIProvider provider)
    : VertexAIChatModel(provider, "gemini-1.0-pro");
}

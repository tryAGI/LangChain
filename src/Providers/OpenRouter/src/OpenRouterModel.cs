using LangChain.Providers.OpenAI;

namespace LangChain.Providers.OpenRouter;

/// <summary>
/// </summary>
public class OpenRouterModel(OpenRouterProvider provider, string id) : OpenAiChatModel(provider, id)
{
    public OpenRouterModel(OpenRouterProvider provider,
        OpenRouterModelIds id) : this(provider, OpenRouterModelProvider.GetModelById(id).Id ?? throw new InvalidOperationException("Model not found"))
    {
    }
}
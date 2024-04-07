using LangChain.Providers.OpenAI;
using OpenAI.Constants;

namespace LangChain.Providers.OpenRouter;

/// <summary>
/// </summary>
public class OpenRouterModel(OpenRouterProvider provider, ChatModels model) : OpenAiChatModel(provider, model)
{
    public OpenRouterModel(OpenRouterProvider provider,
        OpenRouterModelIds id) : this(provider, OpenRouterModelProvider.GetModelById(id))
    {
    }

    public OpenRouterModel(OpenRouterProvider provider, string id) : this(provider, new ChatModels(
        id,
        0,
        PricePerOutputTokenInUsd: 0.0,
        PricePerInputTokenInUsd: 0.0))
    {
    }

   
}
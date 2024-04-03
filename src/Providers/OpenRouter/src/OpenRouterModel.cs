using LangChain.Providers.OpenAI;

namespace LangChain.Providers.OpenRouter;

/// <summary>
/// 
/// </summary>
public class OpenRouterModel:OpenAiChatModel
{
    public OpenRouterModel(OpenRouterProvider provider,
        OpenRouterModelIds id) : base(provider, OpenRouterModelProvider.GetModelById(id))
    {

    }
    public OpenRouterModel(OpenRouterProvider provider, string id):base(provider,id)
    {

    }
}



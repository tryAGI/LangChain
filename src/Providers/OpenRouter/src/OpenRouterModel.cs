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

    #region MyRegion
    
    protected override Task CallFunctionsAsync(global::OpenAI.Chat.Message message, List<Message> messages, CancellationToken cancellationToken = default)
    {
        if (!this.Id.Contains("openai/"))
            throw new NotImplementedException("Function calling is only supported with OpenAI Models.");
        return base.CallFunctionsAsync(message, messages, cancellationToken);
    }

    #endregion
}
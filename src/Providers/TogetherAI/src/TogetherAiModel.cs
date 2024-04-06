using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenRouter;
using OpenAI.Constants;

namespace LangChain.Providers.TogetherAi;

/// <summary>
/// </summary>
public class TogetherAiModel(TogetherAiProvider provider, ChatModels model) : OpenAiChatModel(provider, model)
{
    public TogetherAiModel(TogetherAiProvider provider,
        TogetherAiModelIds id) : this(provider, TogetherAiModelProvider.GetModelById(id))
    {
    }

    public TogetherAiModel(TogetherAiProvider provider, string id) : this(provider, new ChatModels(
        id,
        0,
        PricePerOutputTokenInUsd: 0.0,
        PricePerInputTokenInUsd: 0.0))
    {
    }
}
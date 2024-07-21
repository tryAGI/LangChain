using LangChain.Providers.OpenAI;

namespace LangChain.Providers.TogetherAi;

/// <summary>
/// </summary>
public class TogetherAiModel(TogetherAiProvider provider, string id) : OpenAiChatModel(provider, id)
{
    public TogetherAiModel(TogetherAiProvider provider,
        TogetherAiModelIds id) : this(provider, TogetherAiModelProvider.GetModelById(id).Id ?? throw new InvalidOperationException("Model not found"))
    {
    }
}
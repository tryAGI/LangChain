using LangChain.Providers.OpenAI;

namespace LangChain.Providers.TogetherAi;

/// <summary>
/// </summary>
public class TogetherEmbeddingModel(TogetherAiProvider provider, string id)
    : OpenAiEmbeddingModel(provider, id);
namespace LangChain.Providers.HuggingFace.Predefined;

/// <inheritdoc cref="RecommendedModelIds.Gpt2" />
public class Gpt2Model(HuggingFaceProvider provider)
    : HuggingFaceChatModel(
        provider: provider,
        id: RecommendedModelIds.Gpt2);
namespace LangChain.Providers.HuggingFace.Predefined;

/// <inheritdoc cref="RecommendedModelIds.Gpt2" />
public class Gpt2Model(string apiKey, HttpClient httpClient)
    : HuggingFaceChatModel(
        provider: new HuggingFaceProvider(apiKey, httpClient),
        id: RecommendedModelIds.Gpt2);
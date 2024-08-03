namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="CreateEmbeddingRequestModel.TextEmbeddingAda002" />
public class TextEmbeddingAda002Model(OpenAiProvider provider)
    : OpenAiEmbeddingModel(provider, id: CreateEmbeddingRequestModel.TextEmbeddingAda002);

/// <inheritdoc cref="CreateEmbeddingRequestModel.TextEmbedding3Small" />
public class TextEmbeddingV3SmallModel(OpenAiProvider provider)
    : OpenAiEmbeddingModel(provider, id: CreateEmbeddingRequestModel.TextEmbedding3Small);

/// <inheritdoc cref="CreateEmbeddingRequestModel.TextEmbedding3Large" />
public class TextEmbeddingV3LargeModel(OpenAiProvider provider)
    : OpenAiEmbeddingModel(provider, id: CreateEmbeddingRequestModel.TextEmbedding3Large);
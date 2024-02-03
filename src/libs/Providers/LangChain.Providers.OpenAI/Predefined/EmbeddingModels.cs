using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="EmbeddingModels.Ada002" />
public class TextEmbeddingAda002Model(string apiKey)
    : OpenAiEmbeddingModel(new OpenAiProvider(apiKey), id: EmbeddingModels.Ada002);

/// <inheritdoc cref="EmbeddingModels.Version3Small" />
public class TextEmbeddingV3SmallModel(string apiKey)
    : OpenAiEmbeddingModel(new OpenAiProvider(apiKey), id: EmbeddingModels.Version3Small);

/// <inheritdoc cref="EmbeddingModels.Version3Large" />
public class TextEmbeddingV3LargeModel(string apiKey)
    : OpenAiEmbeddingModel(new OpenAiProvider(apiKey), id: EmbeddingModels.Version3Large);
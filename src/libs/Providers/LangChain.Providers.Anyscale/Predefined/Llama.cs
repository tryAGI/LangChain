namespace LangChain.Providers.Anyscale.Predefined;

/// <inheritdoc cref="ModelIds.Llama2_70B" />
public class Llama2LargeModel(string apiKey)
    : AnyscaleModel(new AnyscaleProvider(apiKey), id: ModelIds.Llama2_70B);

/// <inheritdoc cref="ModelIds.Llama2_13B" />
public class Llama2MediumModel(string apiKey)
    : AnyscaleModel(new AnyscaleProvider(apiKey), id: ModelIds.Llama2_13B);

/// <inheritdoc cref="ModelIds.Llama2_7B" />
public class Llama2SmallModel(string apiKey)
    : AnyscaleModel(new AnyscaleProvider(apiKey), id: ModelIds.Llama2_7B);
    
/// <inheritdoc cref="ModelIds.CodeLlama_34B" />
public class CodeLlamaLargeModel(string apiKey)
    : AnyscaleModel(new AnyscaleProvider(apiKey), id: ModelIds.CodeLlama_34B);
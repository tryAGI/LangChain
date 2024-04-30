namespace LangChain.Providers.Anyscale.Predefined;

/// <inheritdoc cref="ModelIds.Llama2_70B" />
public class Llama2LargeModel(AnyscaleProvider provider)
    : AnyscaleModel(provider, id: ModelIds.Llama2_70B);

/// <inheritdoc cref="ModelIds.Llama2_13B" />
public class Llama2MediumModel(AnyscaleProvider provider)
    : AnyscaleModel(provider, id: ModelIds.Llama2_13B);

/// <inheritdoc cref="ModelIds.Llama2_7B" />
public class Llama2SmallModel(AnyscaleProvider provider)
    : AnyscaleModel(provider, id: ModelIds.Llama2_7B);

/// <inheritdoc cref="ModelIds.CodeLlama_34B" />
public class CodeLlamaLargeModel(AnyscaleProvider provider)
    : AnyscaleModel(provider, id: ModelIds.CodeLlama_34B);
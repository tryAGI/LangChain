namespace LangChain.Providers.Anyscale;

/// <inheritdoc cref="ModelIds.Llama2_7B" />
/// <param name="apiKey"></param>
/// <param name="httpClient"></param>
/// <exception cref="ArgumentNullException"></exception>
public class Llama2SmallModel(string apiKey, HttpClient httpClient)
    : AnyscaleModel(apiKey, httpClient, id: ModelIds.Llama2_7B);
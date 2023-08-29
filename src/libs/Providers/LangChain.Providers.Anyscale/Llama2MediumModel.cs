namespace LangChain.Providers.Anyscale;

/// <inheritdoc cref="ModelIds.Llama2_13B" />
/// <param name="apiKey"></param>
/// <param name="httpClient"></param>
/// <exception cref="ArgumentNullException"></exception>
public class Llama2MediumModel(string apiKey, HttpClient httpClient)
    : AnyscaleModel(apiKey, httpClient, id: ModelIds.Llama2_13B);
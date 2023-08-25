namespace LangChain.Providers;

/// <inheritdoc cref="ModelIds.Llama2_70B" />
/// <param name="apiKey"></param>
/// <param name="httpClient"></param>
/// <exception cref="ArgumentNullException"></exception>
public class Llama2LargeModel(string apiKey, HttpClient httpClient)
    : AnyscaleModel(apiKey, httpClient, id: ModelIds.Llama2_70B);